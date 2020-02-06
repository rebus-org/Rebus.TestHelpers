using System;
using System.Collections.Generic;
using Rebus.Extensions;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Recorded when a message was deferred
    /// </summary>
    public abstract class MessageDeferredToDestination : FakeBusEvent
    {
        internal MessageDeferredToDestination(string destinationAddress, TimeSpan delay, object commandMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
        {
            DestinationAddress = destinationAddress;
            Delay = delay;
            CommandMessage = commandMessage ?? throw new ArgumentNullException(nameof(commandMessage));
            OptionalHeaders = optionalHeaders?.Clone();
        }

        /// <summary>
        /// Gets the destination address to which the message was sent
        /// </summary>
        public string DestinationAddress { get; }

        /// <summary>
        /// Gets the time span with which this message was delayed
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the message that was deferred
        /// </summary>
        public object CommandMessage { get; }

        /// <summary>
        /// Gets the optional headers if they were supplied, or null if they weren't
        /// </summary>
        public Dictionary<string, string> OptionalHeaders { get; }
    }

    /// <summary>
    /// Recorded when a message was deferred
    /// </summary>
    public class MessageDeferredToDestination<TMessage> : MessageDeferredToDestination
    {
        internal MessageDeferredToDestination(string destinationAddress, TimeSpan delay, object commandMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time)
            : base(destinationAddress, delay, commandMessage, optionalHeaders, time)
        {
            CommandMessage = (TMessage)commandMessage;
        }

        /// <summary>
        /// Gets the message that was deferred
        /// </summary>
        public new TMessage CommandMessage { get; }
    }
}