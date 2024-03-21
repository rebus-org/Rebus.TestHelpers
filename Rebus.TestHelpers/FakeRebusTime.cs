using Rebus.Time;
using System;

namespace Rebus.TestHelpers;

/// <summary>
/// Fake implementation
/// </summary>
public class FakeRebusTime : IRebusTime
{
    readonly Random _random = new(DateTime.Now.GetHashCode());

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
                time = time.AddTicks(_random.Next(17) + 1);
            }

            return timeToReturn;
        };
    }

    /// <summary>
    /// Resets the fake <see cref="IRebusTime"/> back to returning <see cref="DateTimeOffset.Now"/>
    /// </summary>
    public void Reset() => _fakeTimeFactory = () => DateTimeOffset.Now;
}