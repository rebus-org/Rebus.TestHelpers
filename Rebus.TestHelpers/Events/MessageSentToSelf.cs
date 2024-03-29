﻿using System;
using System.Collections.Generic;
using Rebus.Extensions;

namespace Rebus.TestHelpers.Events;

/// <summary>
/// Recorded when a message was sent to the bus' own input queue
/// </summary>
public abstract class MessageSentToSelf : FakeBusEvent
{
    internal MessageSentToSelf(object commandMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(time)
    {
        CommandMessage = commandMessage ?? throw new ArgumentNullException(nameof(commandMessage));
        OptionalHeaders = optionalHeaders?.Clone();
    }

    /// <summary>
    /// Gets the message that was sent
    /// </summary>
    public object CommandMessage { get; }

    /// <summary>
    /// Gets the optional headers if they were supplied, or null if they weren't
    /// </summary>
    public Dictionary<string, string> OptionalHeaders { get; }
}

/// <summary>
/// Recorded when a message was sent to the bus' own input queue
/// </summary>
public class MessageSentToSelf<TMessage> : MessageSentToSelf
{
    internal MessageSentToSelf(object commandMessage, Dictionary<string, string> optionalHeaders, DateTimeOffset time) : base(commandMessage, optionalHeaders, time)
    {
        CommandMessage = (TMessage) commandMessage;
    }

    /// <summary>
    /// Gets the message that was sent
    /// </summary>
    public new TMessage CommandMessage { get; }
}