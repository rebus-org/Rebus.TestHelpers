using System.Diagnostics;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Rebus.DataBus;
using Rebus.DataBus.InMem;
using Rebus.TestHelpers.Internals;
using Rebus.Time;

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Implementation of <see cref="IAdvancedApi"/> that can be used to set up fake implementations of various advanced bus APIs for running isolated tests
    /// </summary>
    public class FakeAdvancedApi : IAdvancedApi
    {
        readonly FakeDataBus _dataBus;

        internal FakeAdvancedApi(FakeBusEventRecorder recorder, FakeBusEventFactory factory, IRebusTime rebusTime)
        {
            _dataBus = new FakeDataBus(rebusTime);

            Workers = new FakeWorkersApi(recorder, rebusTime);
            Topics = new FakeTopicsApi(recorder, factory, rebusTime);
            SyncBus = new FakeSyncBus(recorder, factory, rebusTime);
            Routing = new FakeRoutingApi(recorder, factory, rebusTime);
            TransportMessage = new FakeTransportMessageApi(recorder, rebusTime);
        }

        /// <summary>
        /// Gets the workers API if one was passed to the constructor, or throws an exception if that is not the case
        /// </summary>
        public IWorkersApi Workers { get; }

        /// <summary>
        /// Gets the topics API if one was passed to the constructor, or throws an exception if that is not the case
        /// </summary>
        public ITopicsApi Topics { get; }

        /// <summary>
        /// Gets the routing API if one was passed to the constructor, or throws an exception if that is not the case
        /// </summary>
        public IRoutingApi Routing { get; }

        /// <summary>
        /// Gets the transport message API if one was passed to the constructor, or throws an exception if that is not the case
        /// </summary>
        public ITransportMessageApi TransportMessage { get; }

        /// <summary>
        /// Gets the data bus API if one was passed to the constructor, or throws an exception if that is not the case
        /// </summary>
        public IDataBus DataBus => _dataBus;

        /// <summary>
        /// Exposes a synchronous version of <see cref="IBus"/> that essentially mimics all APIs only providing them in an synchronous version
        /// </summary>
        public ISyncBus SyncBus { get; }

        internal InMemDataStore GetDataBusDataStore() => _dataBus.GetDataBusDataStore();
    }
}