using System;

namespace NotUtilities.Queue.Interface
{
    /// <summary>
    /// Defines the contract for a queueing system capable of asynchronous operations for queuing and dequeuing messages.
    /// </summary>
    public interface IQueue : IAsyncDisposable
    {
        /// <summary>
        /// Asynchronously queues a message under a specific topic.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="topic">The topic under which the message will be queued.</param>
        /// <param name="message">The message to be queued.</param>
        /// <param name="token">A cancellation token that can be used to cancel the queuing operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the GUID of the queued message.</returns>
        ValueTask<Guid> QueueMessageAsync<T>(string topic, T message, CancellationToken token = default);

        /// <summary>
        /// Asynchronously dequeues a message of a specific type from a specified topic.
        /// </summary>
        /// <typeparam name="T">The expected type of the dequeued message.</typeparam>
        /// <param name="topic">The topic from which to dequeue the message.</param>
        /// <param name="token">A cancellation token that can be used to cancel the dequeuing operation.</param>
        /// <param name="validateType">A boolean value indicating whether to validate the type of the dequeued message against the specified type <typeparamref name="T"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the dequeued message and its context.</returns>
        ValueTask<(T, IMessageContext)> DequeueMessageAsync<T>(string topic, CancellationToken token = default, bool validateType = true);
    }
}
