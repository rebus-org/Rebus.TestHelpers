using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
#pragma warning disable 1998

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Fake implementation of <see cref="ITransportMessageApi"/>
    /// </summary>
    public class FakeTransportMessageApi : ITransportMessageApi
    {
        readonly FakeBusEventRecorder _recorder;

        internal FakeTransportMessageApi(FakeBusEventRecorder recorder)
        {
            _recorder = recorder;
        }

        /// <inheritdoc />
        public async Task Forward(string destinationAddress, Dictionary<string, string> optionalAdditionalHeaders = null)
        {
            _recorder.Record(new TransportMessageForwarded(destinationAddress, optionalAdditionalHeaders));
        }

        /// <inheritdoc />
        public async Task Defer(TimeSpan delay, Dictionary<string, string> optionalAdditionalHeaders = null)
        {
            _recorder.Record(new TransportMessageDeferred(delay, optionalAdditionalHeaders));
        }
    }
}