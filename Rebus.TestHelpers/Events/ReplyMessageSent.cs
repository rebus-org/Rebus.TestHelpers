using System;
using System.Collections.Generic;
using Rebus.Extensions;

namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Recorded when a reply message was sent
    /// </summary>
    public abstract class ReplyMessageSent : FakeBusEvent
    {
        internal ReplyMessageSent(object replyMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
        {
            ReplyMessage = replyMessage ?? throw new ArgumentNullException(nameof(replyMessage));
            OptionalHeaders = optionalHeaders?.Clone();
        }

        /// <summary>
        /// Gets the message that was sent
        /// </summary>
        public object ReplyMessage { get; }

        /// <summary>
        /// Gets the optional headers if they were supplied, or null if they weren't
        /// </summary>
        public Dictionary<string, string> OptionalHeaders { get; }
    }

    /// <summary>
    /// Recorded when a reply message was sent
    /// </summary>
    public class ReplyMessageSent<TMessage> : ReplyMessageSent
    {
        internal ReplyMessageSent(object replyMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(replyMessage, optionalHeaders, time)
        {
            ReplyMessage = (TMessage) replyMessage;
        }

        /// <summary>
        /// Gets the message that was sent
        /// </summary>
        public new TMessage ReplyMessage { get; }
    }
}