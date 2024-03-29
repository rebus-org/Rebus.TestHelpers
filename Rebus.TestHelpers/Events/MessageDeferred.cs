using System;
using System.Collections.Generic;
using Rebus.Extensions;

namespace Rebus.TestHelpers.Events;

/// <summary>
/// Recorded when a message was deferred
/// </summary>
public abstract class MessageDeferred : FakeBusEvent
{
    internal MessageDeferred(TimeSpan delay, object commandMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
    {
        Delay = delay;
        CommandMessage = commandMessage ?? throw new ArgumentNullException(nameof(commandMessage));
        OptionalHeaders = optionalHeaders?.Clone();
    }

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
public class MessageDeferred<TMessage> : MessageDeferred
{
    internal MessageDeferred(TimeSpan delay, object commandMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) 
        : base(delay, commandMessage, optionalHeaders, time)
    {
        CommandMessage = (TMessage) commandMessage;
    }

    /// <summary>
    /// Gets the message that was deferred
    /// </summary>
    public new TMessage CommandMessage { get; }
}