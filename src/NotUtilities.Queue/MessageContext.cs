using NotUtilities.Queue.Interface;

namespace NotUtilities.Queue
{
    /// <summary>
    /// Provides context for a queued message, including a unique identifier, topic, and the time it was queued.
    /// </summary>
    internal struct MessageContext : IMessageContext
    {
        /// <summary>
        /// Gets or sets the unique identifier for the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the topic associated with the message.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when the message was queued.
        /// </summary>
        public DateTime TimeQueued { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContext"/> struct with a specific topic.
        /// Automatically generates a new GUID for <see cref="Id"/> and sets <see cref="TimeQueued"/> to the current UTC time.
        /// </summary>
        /// <param name="topic">The topic associated with the message.</param>
        public MessageContext(string topic)
        {
            Id = Guid.NewGuid(); // Assign a new unique identifier
            Topic = topic; // Set the topic
            TimeQueued = DateTime.UtcNow; // Set the time the message was queued to the current UTC time
        }
    }
}
