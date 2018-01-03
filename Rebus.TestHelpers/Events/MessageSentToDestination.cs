using System;
using System.Collections.Generic;
using Rebus.Extensions;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Base event recorded when a message was sent to a specific address - actual event will be <see cref="MessageSent{TMessage}"/>
    /// </summary>
    public abstract class MessageSentToDestination : FakeBusEvent
    {
        internal MessageSentToDestination(string destinationAddress, object commandMessage, Dictionary<string, string> optionalHeaders)
        {
            DestinationAddress = destinationAddress;
            OptionalHeaders = optionalHeaders?.Clone();
            CommandMessage = commandMessage ?? throw new ArgumentNullException(nameof(commandMessage));
        }

        /// <summary>
        /// Gets the destination address to which the message was sent
        /// </summary>
        public string DestinationAddress { get; }

        /// <summary>
        /// Gets the optional headers if they were supplied, or null if they weren't
        /// </summary>
        public Dictionary<string, string> OptionalHeaders { get; }

        /// <summary>
        /// Gets the message that was sent
        /// </summary>
        public object CommandMessage { get; }

    }

    /// <summary>
    /// Recorded when a message was sent
    /// </summary>
    public class MessageSentToDestination<TMessage> : MessageSentToDestination
    {
        internal MessageSentToDestination(string destinationAddress, object commandMessage, Dictionary<string, string> optionalHeaders)
            : base(destinationAddress, commandMessage, optionalHeaders)
        {
            CommandMessage = (TMessage)commandMessage;
        }

        /// <summary>
        /// Gets the message that was sent
        /// </summary>
        public new TMessage CommandMessage { get; }
    }
}