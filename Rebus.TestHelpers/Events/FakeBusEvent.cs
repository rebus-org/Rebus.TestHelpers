using System;
using Rebus.Time;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Base type of all events that a <see cref="FakeBus"/> can record.
    /// </summary>
    public abstract class FakeBusEvent
    {
        /// <summary>
        /// Gets the time of when the event was recorded
        /// </summary>
        public DateTimeOffset Time { get; }

        internal FakeBusEvent(DateTimeOffset time) {
            Time = time;
        }

        /// <summary>
        /// Gets a nice string representation of this particular fake bus event
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}