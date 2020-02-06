using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus.Advanced;
using Rebus.Routing;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
using Rebus.Time;
#pragma warning disable 1998

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Fake implementation of <see cref="IRoutingApi"/>
    /// </summary>
    public class FakeRoutingApi : IRoutingApi
    {
        readonly FakeBusEventRecorder _recorder;
        readonly FakeBusEventFactory _factory;
        readonly IRebusTime _rebusTime;

        internal FakeRoutingApi(FakeBusEventRecorder recorder, FakeBusEventFactory factory, IRebusTime rebusTime)
        {
            _recorder = recorder;
            _factory = factory;
            _rebusTime = rebusTime;
        }

        /// <inheritdoc />
        public async Task Send(string destinationAddress, object explicitlyRoutedMessage, IDictionary<string, string> optionalHeaders = null)
        {
            var messageSentToDestination = _factory.CreateEventGeneric<MessageSentToDestination>(
                typeof(MessageSentToDestination<>),
                explicitlyRoutedMessage.GetType(),
                destinationAddress,
                explicitlyRoutedMessage,
                optionalHeaders, 
                _rebusTime.Now
            );

            _recorder.Record(messageSentToDestination);
        }

        /// <inheritdoc />
        public async Task SendRoutingSlip(Itinerary itinerary, object message, IDictionary<string, string> optionalHeaders = null)
        {
            var messageSentWithRoutingSlip = _factory.CreateEventGeneric<MessageSentWithRoutingSlip>(
                typeof(MessageSentWithRoutingSlip<>),
                message.GetType(),
                itinerary,
                message,
                optionalHeaders, 
                _rebusTime.Now
            );

            _recorder.Record(messageSentWithRoutingSlip);
        }

        /// <inheritdoc />
        public async Task Defer(string destinationAddress, TimeSpan delay, object explicitlyRoutedMessage, IDictionary<string, string> optionalHeaders = null)
        {
            var messageSentToDestination = _factory.CreateEventGeneric<MessageDeferredToDestination>(
                typeof(MessageDeferredToDestination<>),
                explicitlyRoutedMessage.GetType(),
                destinationAddress,
                delay,
                explicitlyRoutedMessage,
                optionalHeaders,
                _rebusTime.Now
            );

            _recorder.Record(messageSentToDestination);
        }
    }
}
