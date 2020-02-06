using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;
using Rebus.Time;

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Fake implementation of <see cref="IWorkersApi"/> that records events
    /// </summary>
    public class FakeWorkersApi : IWorkersApi
    {
        readonly FakeBusEventRecorder _recorder;
        private readonly IRebusTime _rebusTime;
        int _currentNumberOfWorkers = 1;

        internal FakeWorkersApi(FakeBusEventRecorder recorder, IRebusTime rebusTime)
        {
            _recorder = recorder;
            _rebusTime = rebusTime;
        }

        /// <inheritdoc />
        public void SetNumberOfWorkers(int numberOfWorkers)
        {
            _currentNumberOfWorkers = numberOfWorkers;
            _recorder.Record(new NumberOfWorkersChanged(numberOfWorkers, _rebusTime.Now));
        }

        /// <inheritdoc />
        public int Count => _currentNumberOfWorkers;
    }
}