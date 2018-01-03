namespace Rebus.TestHelpers.Events
{
    /// <summary>
    /// Event raised when the number of worker threads is changed
    /// </summary>
    public class NumberOfWorkersChanged : FakeBusEvent
    {
        internal NumberOfWorkersChanged(int count)
        {
            Count = count;
        }

        /// <summary>
        /// Gets the new number of workers
        /// </summary>
        public int Count { get; }
    }
}