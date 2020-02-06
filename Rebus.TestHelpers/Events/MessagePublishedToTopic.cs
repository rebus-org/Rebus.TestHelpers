using System;
using System.Collections.Generic;
using Rebus.Extensions;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Recorded when an event was published to a specific topic
    /// </summary>
    public abstract class MessagePublishedToTopic : FakeBusEvent
    {
        /// <summary>
        /// Gets the topic that the event was published to
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// Gets the event message that was published
        /// </summary>
        public object EventMessage { get; }

        /// <summary>
        /// Gets the optional headers if they were supplied, or null if they weren't
        /// </summary>
        public Dictionary<string, string> OptionalHeaders { get; }

        internal MessagePublishedToTopic(string topic, object eventMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
            EventMessage = eventMessage ?? throw new ArgumentNullException(nameof(eventMessage));
            OptionalHeaders = optionalHeaders?.Clone();
        }
    }

    /// <summary>
    /// Recorded when an event was published to a specific topic
    /// </summary>
    public class MessagePublishedToTopic<TMessage> : MessagePublishedToTopic
    {
        internal MessagePublishedToTopic(string topic, object eventMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) 
            : base(topic, eventMessage, optionalHeaders, time)
        {
            EventMessage = (TMessage) eventMessage;
        }

        /// <summary>
        /// Gets the event message that was published
        /// </summary>
        public new TMessage EventMessage { get; }
    }
}