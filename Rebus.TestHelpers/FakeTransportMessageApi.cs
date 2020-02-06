using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
using Rebus.Time;
#pragma warning disable 1998

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Fake implementation of <see cref="ITransportMessageApi"/>
    /// </summary>
    public class FakeTransportMessageApi : ITransportMessageApi
    {
        readonly FakeBusEventRecorder _recorder;
        private readonly IRebusTime _rebusTime;

        internal FakeTransportMessageApi(FakeBusEventRecorder recorder, IRebusTime rebusTime)
        {
            _recorder = recorder;
            _rebusTime = rebusTime;
        }

        /// <inheritdoc />
        public async Task Forward(string destinationAddress, Dictionary<string, string> optionalAdditionalHeaders = null)
        {
            _recorder.Record(new TransportMessageForwarded(destinationAddress, optionalAdditionalHeaders, _rebusTime.Now));
        }

        /// <inheritdoc />
        public async Task Defer(TimeSpan delay, Dictionary<string, string> optionalAdditionalHeaders = null)
        {
            _recorder.Record(new TransportMessageDeferred(delay, optionalAdditionalHeaders, _rebusTime.Now));
        }
    }
}