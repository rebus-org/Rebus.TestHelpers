using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
using Rebus.Time;
#pragma warning disable 1998

namespace Rebus.TestHelpers;

/// <summary>
/// Fake implementation of <see cref="ITopicsApi"/>
/// </summary>
public class FakeTopicsApi : ITopicsApi
{
    readonly FakeBusEventRecorder _recorder;
    readonly FakeBusEventFactory _factory;
    readonly IRebusTime _rebusTime;

    internal FakeTopicsApi(FakeBusEventRecorder recorder, FakeBusEventFactory factory, IRebusTime rebusTime)
    {
        _recorder = recorder;
        _factory = factory;
        _rebusTime = rebusTime;
    }

    /// <inheritdoc />
    public async Task Publish(string topic, object eventMessage, IDictionary<string, string> optionalHeaders = null)
    {
        var messagePublishedToTopicEvent = _factory.CreateEventGeneric<MessagePublishedToTopic>(
            typeof(MessagePublishedToTopic<>),
            eventMessage.GetType(),
            topic,
            eventMessage,
            optionalHeaders,
            _rebusTime.Now
        );

        _recorder.Record(messagePublishedToTopicEvent);
    }

    /// <inheritdoc />
    public async Task Subscribe(string topic) => _recorder.Record(new SubscribedToTopic(topic, _rebusTime.Now));

    /// <inheritdoc />
    public async Task Unsubscribe(string topic) => _recorder.Record(new UnsubscribedFromTopic(topic, _rebusTime.Now));
}