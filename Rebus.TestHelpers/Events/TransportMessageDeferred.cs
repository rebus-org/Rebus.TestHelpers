using System;
using System.Collections.Generic;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Event emitted when the transport message currently being handled is forwarded to another destination address
    /// </summary>
    public class TransportMessageDeferred : FakeBusEvent
    {
        /// <summary>
        /// Gets the time span with which this message was delayed
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the optional headers if they were supplied, or null if they weren't
        /// </summary>
        public Dictionary<string, string> OptionalHeaders { get; }

        internal TransportMessageDeferred(TimeSpan delay, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
        {
            Delay = delay;
            OptionalHeaders = optionalHeaders;
        }
    }
}