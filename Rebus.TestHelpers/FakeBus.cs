using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Rebus.DataBus;
using Rebus.DataBus.InMem;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
using Rebus.Time;

#pragma warning disable 1998

namespace Rebus.TestHelpers
{
    /// <summary>
    /// The fake bus is an implementation of <see cref="IBus"/> that can be used for testing. The fake bus
    /// just collects information about what has happened to it, allowing you to query that information after the
    /// fact by checking <see cref="Events"/>
    /// </summary>
    public class FakeBus : IBus
    {
        readonly FakeBusEventRecorder _recorder = new FakeBusEventRecorder();
        readonly FakeBusEventFactory _factory = new FakeBusEventFactory();
        readonly FakeAdvancedApi _advanced;
        readonly IRebusTime _rebusTime;

        /// <summary>
        /// Creates the fake <see cref="IBus"/> implementation
        /// </summary>
        public FakeBus()
        {
            _rebusTime = new FakeRebusTime();
            _advanced = new FakeAdvancedApi(_recorder, _factory, _rebusTime);
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
        public async Task SendLocal(object commandMessage, IDictionary<string, string> optionalHeaders = null)
        {
            var messageSentToSelfEvent = _factory.CreateEventGeneric<MessageSentToSelf>(typeof(MessageSentToSelf<>), commandMessage.GetType(), commandMessage, optionalHeaders, _rebusTime.Now);

            Record(messageSentToSelfEvent);
        }

        /// <inheritdoc />
        public async Task Send(object commandMessage, IDictionary<string, string> optionalHeaders = null)
        {
            var messageSentEvent = _factory.CreateEventGeneric<MessageSent>(typeof(MessageSent<>), commandMessage.GetType(), commandMessage, optionalHeaders, _rebusTime.Now);

            Record(messageSentEvent);
        }

        /// <inheritdoc />
        public async Task Reply(object replyMessage, IDictionary<string, string> optionalHeaders = null)
        {
            var replyMessageSentEvent = _factory.CreateEventGeneric<ReplyMessageSent>(typeof(ReplyMessageSent<>), replyMessage.GetType(), replyMessage, optionalHeaders, _rebusTime.Now);

            Record(replyMessageSentEvent);
        }

        /// <inheritdoc />
        public async Task Defer(TimeSpan delay, object message, IDictionary<string, string> optionalHeaders = null)
        {
            var messageDeferredEvent = _factory.CreateEventGeneric<MessageDeferred>(typeof(MessageDeferred<>), message.GetType(), delay, message, optionalHeaders, _rebusTime.Now);

            Record(messageDeferredEvent);
        }

        /// <inheritdoc />
        public async Task DeferLocal(TimeSpan delay, object message, IDictionary<string, string> optionalHeaders = null)
        {
            var messageDeferredEvent = _factory.CreateEventGeneric<MessageDeferredToSelf>(typeof(MessageDeferredToSelf<>), message.GetType(), delay, message, optionalHeaders, _rebusTime.Now);

            Record(messageDeferredEvent);
        }

        /// <summary>
        /// Gets the advanced API
        /// </summary>
        public IAdvancedApi Advanced
        {
            get { return _advanced; }
        }

        /// <inheritdoc />
        public async Task Subscribe<TEvent>()
        {
            Record(new Subscribed(typeof(TEvent), _rebusTime.Now));
        }

        /// <inheritdoc />
        public async Task Subscribe(Type eventType)
        {
            Record(new Subscribed(eventType, _rebusTime.Now));
        }

        /// <inheritdoc />
        public async Task Unsubscribe<TEvent>()
        {
            Record(new Unsubscribed(typeof(TEvent), _rebusTime.Now));
        }

        /// <inheritdoc />
        public async Task Unsubscribe(Type eventType)
        {
            Record(new Unsubscribed(eventType, _rebusTime.Now));
        }

        /// <inheritdoc />
        public async Task Publish(object eventMessage, IDictionary<string, string> optionalHeaders = null)
        {
            var messagePublishedEvent = _factory.CreateEventGeneric<MessagePublished>(typeof(MessagePublished<>), eventMessage.GetType(), eventMessage, optionalHeaders, _rebusTime.Now);

            Record(messagePublishedEvent);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Record(new FakeBusDisposed(_rebusTime.Now));
        }

        void Record(FakeBusEvent fakeBusEvent)
        {
            _recorder.Record(fakeBusEvent);
        }

        /// <summary>
        /// Gets the data bus storage
        /// </summary>
        public InMemDataStore GetDataBusDataStore() => _advanced.GetDataBusDataStore();
    }
}