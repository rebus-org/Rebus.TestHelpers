using System;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Recorded when a subscription was revoked from a specific topic
    /// </summary>
    public class UnsubscribedFromTopic : FakeBusEvent
    {
        /// <summary>
        /// Gets the topic
        /// </summary>
        public string Topic { get; }

        internal UnsubscribedFromTopic(string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }
    }
}