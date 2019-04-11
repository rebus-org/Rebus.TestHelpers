using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus.Advanced;
using Rebus.Routing;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
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

        internal FakeRoutingApi(FakeBusEventRecorder recorder, FakeBusEventFactory factory)
        {
            _recorder = recorder;
            _factory = factory;
        }

        /// <inheritdoc />
        public async Task Send(string destinationAddress, object explicitlyRoutedMessage, Dictionary<string, string> optionalHeaders = null)
        {
            var messageSentToDestination = _factory.CreateEventGeneric<MessageSentToDestination>(
                typeof(MessageSentToDestination<>),
                explicitlyRoutedMessage.GetType(),
                destinationAddress, 
                explicitlyRoutedMessage,
                optionalHeaders
            );

            _recorder.Record(messageSentToDestination);
        }

        /// <inheritdoc />
        public async Task SendRoutingSlip(Itinerary itinerary, object message, Dictionary<string, string> optionalHeaders = null)
        {
            var messageSentWithRoutingSlip = _factory.CreateEventGeneric<MessageSentWithRoutingSlip>(
                typeof(MessageSentWithRoutingSlip<>),
                message.GetType(),
                itinerary,
                message,
                optionalHeaders
            );

            _recorder.Record(messageSentWithRoutingSlip);
        }

        /// <inheritdoc />
        public async Task Defer(string destinationAddress, TimeSpan delay, object explicitlyRoutedMessage, Dictionary<string, string> optionalHeaders = null)
        {
            var messageSentToDestination = _factory.CreateEventGeneric<MessageDeferredToDestination>(
                typeof(MessageDeferredToDestination<>),
                explicitlyRoutedMessage.GetType(),
                destinationAddress,
                delay,
                explicitlyRoutedMessage,
                optionalHeaders
            );

            _recorder.Record(messageSentToDestination);
        }
    }
}
