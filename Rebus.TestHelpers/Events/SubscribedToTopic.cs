using System;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Recorded when a subscription was made to a specific topic
    /// </summary>
    public class SubscribedToTopic : FakeBusEvent
    {
        /// <summary>
        /// Gets the topic
        /// </summary>
        public string Topic { get; }

        internal SubscribedToTopic(string topic, DateTimeOffset time) : base(time)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }
    }
}