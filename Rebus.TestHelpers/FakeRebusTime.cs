using Rebus.Time;
using System;

namespace Rebus.TestHelpers;

/// <summary>
/// Fake implementation
/// </summary>
public class FakeRebusTime : IRebusTime
{
    Func<DateTimeOffset> _fakeTimeFactory = () => DateTimeOffset.Now;

    /// <inheritdoc />
    public DateTimeOffset Now => _fakeTimeFactory();

    /// <summary>
    /// Set Now to a new value
    /// </summary>
    /// <param name="fakeTime"></param>
    /// <param name="driftSlightly"></param>
    public void FakeIt(DateTimeOffset fakeTime, bool driftSlightly = true)
    {
        var time = fakeTime;

        _fakeTimeFactory = () =>
        {
            var timeToReturn = time;
            if (driftSlightly)
            {
                time = time.AddTicks(17);
            }

            return timeToReturn;
        };
    }

    /// <inheritdoc />
    public void Reset() => _fakeTimeFactory = () => DateTimeOffset.Now;
}