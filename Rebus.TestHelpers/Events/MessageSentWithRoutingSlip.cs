using System;
using System.Collections.Generic;
using Rebus.Extensions;
using Rebus.Routing;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Base event recorded when a message was sent to a specific address - actual event will be <see cref="MessageSent{TMessage}"/>
    /// </summary>
    public abstract class MessageSentWithRoutingSlip : FakeBusEvent
    {
        internal MessageSentWithRoutingSlip(Itinerary itinerary, object commandMessage, Dictionary<string, string> optionalHeaders)
        {
            Itinerary = itinerary;
            OptionalHeaders = optionalHeaders?.Clone();
            CommandMessage = commandMessage ?? throw new ArgumentNullException(nameof(commandMessage));
        }

        /// <summary>
        /// Gets the planned destinations for this message
        /// </summary>
        public Itinerary Itinerary { get; }

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
    public class MessageSentWithRoutingSlip<TMessage> : MessageSentWithRoutingSlip
    {
        internal MessageSentWithRoutingSlip(Itinerary itinerary, object commandMessage, Dictionary<string, string> optionalHeaders)
            : base(itinerary, commandMessage, optionalHeaders)
        {
            CommandMessage = (TMessage)commandMessage;
        }

        /// <summary>
        /// Gets the message that was sent
        /// </summary>
        public new TMessage CommandMessage { get; }
    }
}