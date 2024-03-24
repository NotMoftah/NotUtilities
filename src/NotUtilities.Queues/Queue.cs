using NotUtilities.Core.Abstraction;
using NotUtilities.Queues.Interface;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace NotUtilities.Queues
{
    /// <summary>
    /// Represents an object queued for processing, including its message and associated context.
    /// </summary>
    internal record QueuedObject
    {
        public object Message { get; init; }
        public MessageContext Context { get; init; }
    }

    /// <summary>
    /// Implements a queue system with topic-based message queuing.
    /// </summary>
    public class Queue : AsyncDisposableResource, IQueue
    {
        private readonly int _capacity;
        private readonly ConcurrentDictionary<string, Channel<QueuedObject>> _queues;

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class with a specified capacity for each topic's queue.
        /// </summary>
        /// <param name="capacity">The maximum number of messages each topic queue can hold.</param>
        public Queue(int capacity)
        {
            _capacity = capacity;
            _queues = new ConcurrentDictionary<string, Channel<QueuedObject>>();
        }

        /// <summary>
        /// Asynchronously queues a message under a specific topic.
        /// </summary>
        /// <typeparam name="T">The type of the message to be queued.</typeparam>
        /// <param name="topic">The topic under which the message is to be queued.</param>
        /// <param name="message">The message to queue.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <returns>A task that represents the asynchronous operation, containing a GUID for the queued message.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the topic or message is null or whitespace.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the Queue is disposed.</exception>
        public async ValueTask<Guid> QueueMessageAsync<T>(string topic, T message, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentNullException(nameof(topic), "Topic cannot be null or whitespace.");

            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            var queue = GetQueueOfTopic(topic.ToLowerInvariant());
            var queuedObject = new QueuedObject { Message = message, Context = new MessageContext(topic) };

            await queue.Writer.WaitToWriteAsync(token);
            await queue.Writer.WriteAsync(queuedObject, token);

            return queuedObject.Context.Id;
        }

        /// <summary>
        /// Asynchronously dequeues a message of a specified type from a specified topic.
        /// </summary>
        /// <typeparam name="T">The expected type of the dequeued message.</typeparam>
        /// <param name="topic">The topic from which to dequeue a message.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <param name="validateType">Indicates whether to validate the type of the dequeued message.</param>
        /// <returns>A task that represents the asynchronous operation, containing the dequeued message and its context.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the topic is null or whitespace.</exception>
        /// <exception cref="InvalidCastException">Thrown if the dequeued message does not match the specified type and type validation is enabled.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the Queue is disposed.</exception>
        public async ValueTask<(T, IMessageContext)> DequeueMessageAsync<T>(string topic, CancellationToken token = default, bool validateType = true)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentNullException(nameof(topic), "Topic cannot be null or whitespace.");

            var queue = GetQueueOfTopic(topic.ToLowerInvariant());

            await queue.Reader.WaitToReadAsync(token);
            var result = await queue.Reader.ReadAsync(token);

            if (validateType && !(result.Message is T))
                throw new InvalidCastException($"Expected message of type {typeof(T).Name}, but got {result.Message.GetType().Name}.");

            return ((T)result.Message, result.Context);
        }

        /// <summary>
        /// Gets or creates a channel for a specific topic.
        /// </summary>
        /// <param name="topic">The topic for which to get or create a channel.</param>
        /// <returns>The channel associated with the specified topic.</returns>
        private Channel<QueuedObject> GetQueueOfTopic(string topic)
        {
            if (base.disposed)
                throw new ObjectDisposedException(nameof(Queue));

            return _queues.GetOrAdd(topic, _ => Channel.CreateBounded<QueuedObject>(_capacity));
        }

        /// <summary>
        /// Dispose all the queues.
        /// </summary>
        public override async ValueTask DisposeAsync()
        {
            foreach (var queue in _queues.Values)
            {
                // Complete the writer to signal no more writes will be attempted.
                queue.Writer.Complete();

                try
                {
                    // Drain the queue of remaining items.
                    while (await queue.Reader.WaitToReadAsync())
                    {
                        // Attempt to read and discard the remaining items. This loop ensures all queued items are acknowledged,
                        // which is important for avoiding memory leaks by ensuring that items do not remain indefinitely in the channel.
                        while (queue.Reader.TryRead(out var _)) { }
                    }
                }
                catch (ChannelClosedException)
                {
                    // This exception is thrown if the channel is already completed or disposed.
                    // It's safe to ignore this exception since we're in the process of disposing.
                }
            }

            _queues.Clear();

            await base.DisposeAsync();
        }
    }
}
