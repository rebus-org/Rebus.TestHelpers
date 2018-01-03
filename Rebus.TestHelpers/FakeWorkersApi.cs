using Rebus.Bus.Advanced;
using Rebus.TestHelpers.Events;
using Rebus.TestHelpers.Internals;

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Fake implementation of <see cref="IWorkersApi"/> that records events
    /// </summary>
    public class FakeWorkersApi : IWorkersApi
    {
        readonly FakeBusEventRecorder _recorder;

        int _currentNumberOfWorkers = 1;

        internal FakeWorkersApi(FakeBusEventRecorder recorder)
        {
            _recorder = recorder;
        }

        /// <inheritdoc />
        public void SetNumberOfWorkers(int numberOfWorkers)
        {
            _currentNumberOfWorkers = numberOfWorkers;
            _recorder.Record(new NumberOfWorkersChanged(numberOfWorkers));
        }

        /// <inheritdoc />
        public int Count => _currentNumberOfWorkers;
    }
}