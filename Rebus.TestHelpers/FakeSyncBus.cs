﻿using System;
using System.Collections.Generic;
using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
using Rebus.Time;
#pragma warning disable 1998

namespace Rebus.TestHelpers;

/// <summary>
/// The fake sync bus is an implementation of <see cref="ISyncBus"/> that can be used for testing. The fake bus
/// just collects information about what has happened to it, allowing you to query that information after the
/// fact by checking <see cref="Events"/>
/// </summary>
public class FakeSyncBus : ISyncBus
{
    readonly FakeBusEventRecorder _recorder;
    readonly FakeBusEventFactory _factory;
    readonly IRebusTime _rebusTime;

    /// <summary>
    /// Creates the fake sync bus with its own internal event recorder (may be used if only an <see cref="ISyncBus"/> 
    /// implementation is needed for the test)
    /// </summary>
    public FakeSyncBus()
    {
        _recorder = new FakeBusEventRecorder();
        _factory = new FakeBusEventFactory();
        _rebusTime = new FakeRebusTime();
    }

    internal FakeSyncBus(FakeBusEventRecorder recorder, FakeBusEventFactory factory, IRebusTime rebusTime)
    {
        _recorder = recorder;
        _factory = factory;
        _rebusTime = rebusTime;
    }

    /// <summary>
    /// Gets all events recorded at this point. Query this in order to check what happened to the fake bus while
    /// it participated in a test - e.g. like this:
    /// <code>
    /// await fakeBus.Send(new MyMessage("woohoo!"));
    ///
    /// var sentMessagesWithMyGreeting = fakeBus.Events
    ///     .OfType&lt;MessageSent&lt;MyMessage&gt;&gt;()
    ///     .Count(m => m.CommandMessage.Text == "woohoo!");
    ///
    /// Assert.That(sentMessagesWithMyGreeting, Is.EqualTo(1));
    /// </code>
    /// </summary>
    public IEnumerable<FakeBusEvent> Events => _recorder.GetEvents();

    /// <summary>
    /// Adds a callback to be invoked when new events are recorded in the fake bus
    /// </summary>
    public void On<TEvent>(Action<TEvent> callback) where TEvent : FakeBusEvent
    {
        _recorder.AddCallback(callback);
    }

    /// <summary>
    /// Clears all events recorded by the fake bus. Registered callbacks will NOT be cleared
    /// </summary>
    public void Clear()
    {
        _recorder.Clear();
    }
        
    /// <inheritdoc />
    public void SendLocal(object commandMessage, IDictionary<string, string> optionalHeaders = null)
    {
        var messageSentToSelfEvent = _factory.CreateEventGeneric<MessageSentToSelf>(typeof(MessageSentToSelf<>), commandMessage.GetType(), commandMessage, optionalHeaders, _rebusTime.Now);

        Record(messageSentToSelfEvent);
    }

    /// <inheritdoc />
    public void Send(object commandMessage, IDictionary<string, string> optionalHeaders = null)
    {
        var messageSentEvent = _factory.CreateEventGeneric<MessageSent>(typeof(MessageSent<>), commandMessage.GetType(), commandMessage, optionalHeaders, _rebusTime.Now);

        Record(messageSentEvent);
    }

    /// <inheritdoc />
    public void Reply(object replyMessage, IDictionary<string, string> optionalHeaders = null)
    {
        var replyMessageSentEvent = _factory.CreateEventGeneric<ReplyMessageSent>(typeof(ReplyMessageSent<>), replyMessage.GetType(), replyMessage, optionalHeaders, _rebusTime.Now);

        Record(replyMessageSentEvent);
    }

    /// <inheritdoc />
    public void Defer(TimeSpan delay, object message, IDictionary<string, string> optionalHeaders = null)
    {
        var messageDeferredEvent = _factory.CreateEventGeneric<MessageDeferred>(typeof(MessageDeferred<>), message.GetType(), delay, message, optionalHeaders, _rebusTime.Now);

        Record(messageDeferredEvent);
    }

    /// <inheritdoc />
    public void DeferLocal(TimeSpan delay, object message, IDictionary<string, string> optionalHeaders = null)
    {
        var messageDeferredEvent = _factory.CreateEventGeneric<MessageDeferredToSelf>(typeof(MessageDeferredToSelf<>), message.GetType(), delay, message, optionalHeaders, _rebusTime.Now);

        Record(messageDeferredEvent);
    }

    /// <inheritdoc />
    public void Subscribe<TEvent>()
    {
        Record(new Subscribed(typeof(TEvent), _rebusTime.Now));
    }

    /// <inheritdoc />
    public void Subscribe(Type eventType)
    {
        Record(new Subscribed(eventType, _rebusTime.Now));
    }

    /// <inheritdoc />
    public void Unsubscribe<TEvent>()
    {
        Record(new Unsubscribed(typeof(TEvent), _rebusTime.Now));
    }

    /// <inheritdoc />
    public void Unsubscribe(Type eventType)
    {
        Record(new Unsubscribed(eventType, _rebusTime.Now));
    }

    /// <inheritdoc />
    public void Publish(object eventMessage, IDictionary<string, string> optionalHeaders = null)
    {
        var messagePublishedEvent = _factory.CreateEventGeneric<MessagePublished>(typeof(MessagePublished<>), eventMessage.GetType(), eventMessage, optionalHeaders, _rebusTime.Now);

        Record(messagePublishedEvent);
    }

    void Record(FakeBusEvent fakeBusEvent)
    {
        _recorder.Record(fakeBusEvent);
    }
}