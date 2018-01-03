using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
#pragma warning disable 1998

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Fake implementation of <see cref="ITopicsApi"/>
    /// </summary>
    public class FakeTopicsApi : ITopicsApi
    {
        readonly FakeBusEventRecorder _recorder;
        readonly FakeBusEventFactory _factory;

        internal FakeTopicsApi(FakeBusEventRecorder recorder, FakeBusEventFactory factory)
        {
            _recorder = recorder;
            _factory = factory;
        }

        /// <inheritdoc />
        public async Task Publish(string topic, object eventMessage, Dictionary<string, string> optionalHeaders = null)
        {
            var messagePublishedToTopicEvent = _factory.CreateEventGeneric<MessagePublishedToTopic>(
                typeof(MessagePublishedToTopic<>),
                eventMessage.GetType(),
                topic,
                eventMessage,
                optionalHeaders
            );

            _recorder.Record(messagePublishedToTopicEvent);
        }

        /// <inheritdoc />
        public async Task Subscribe(string topic) => _recorder.Record(new SubscribedToTopic(topic));

        /// <inheritdoc />
        public async Task Unsubscribe(string topic) => _recorder.Record(new UnsubscribedFromTopic(topic));
    }
}