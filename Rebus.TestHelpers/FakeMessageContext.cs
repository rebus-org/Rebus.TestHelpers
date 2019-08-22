using System;
using System.Collections.Generic;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
// ReSharper disable UnusedMember.Global

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Implementation of <see cref="IMessageContext"/> that can be used when unit testing handlers.
    /// </summary>
    public class FakeMessageContext : IMessageContext
    {
        /// <summary>
        /// Creates the fake message context with the specified <paramref name="message"/>, <paramref name="transportMessage"/>, and <paramref name="headers"/>.
        /// Please note that they can all be replaced after creation
        /// </summary>
        public FakeMessageContext(Message message = null, TransportMessage transportMessage = null, Dictionary<string, string> headers = null)
        {
            Message = message;
            TransportMessage = transportMessage;
            Headers = headers ?? message?.Headers ?? transportMessage?.Headers ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/>, because the fake message context does not have a <see cref="ITransactionContext"/>
        /// </summary>
        public ITransactionContext TransactionContext => throw new InvalidOperationException("The fake message context does not have a valid transaction context, sorry");

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/>, because the fake message context does not have a <see cref="IncomingStepContext"/>
        /// </summary>
        public IncomingStepContext IncomingStepContext => throw new InvalidOperationException("The fake message context does not have a valid incoming step context, sorry");

        /// <summary>
        /// Gets the transport message if one was passed to the constructor, null otherwise
        /// </summary>
        public TransportMessage TransportMessage { get; set; }

        /// <summary>
        /// Gets the logical message if one was passed to the constructor, null otherwise
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Gets the message headers if any could be found either explicitly passed to the constructor, or via the logical message, or via the transport message. Empty dictionary otherwise
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
    }
}