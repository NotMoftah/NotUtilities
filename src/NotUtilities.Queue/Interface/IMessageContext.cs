using System;

namespace NotUtilities.Queue.Interface
{
    /// <summary>
    /// Defines the contract for a message context, which includes the message's unique identifier, its topic, and the time it was queued.
    /// </summary>
    public interface IMessageContext
    {
        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the topic associated with the message.
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// Gets the UTC time when the message was queued.
        /// </summary>
        DateTime TimeQueued { get; }
    }
}
