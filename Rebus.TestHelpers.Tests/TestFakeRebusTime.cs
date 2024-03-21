using System;
using NUnit.Framework;

namespace Rebus.TestHelpers.Tests;

[TestFixture]
public class TestFakeRebusTime
{
    [Test]
    public void DefaultsToCurrentTime()
    {
        var fakeRebusTime = new FakeRebusTime();

        var fakeNow = fakeRebusTime.Now;
        var actualNow = DateTimeOffset.Now;

        Assert.That(Math.Abs((fakeNow - actualNow).TotalMilliseconds), Is.LessThan(10));
    }

    [Test]
    public void CanBeFixed()
    {
        var fakeRebusTime = new FakeRebusTime();

        var fakeTime = new DateTimeOffset(2024, 3, 19, 14, 00, 00, TimeSpan.FromHours(1));

        fakeRebusTime.FakeIt(fakeTime, driftSlightly: false);

        Assert.That(fakeRebusTime.Now, Is.EqualTo(fakeTime));
        Assert.That(fakeRebusTime.Now, Is.EqualTo(fakeTime));
        Assert.That(fakeRebusTime.Now, Is.EqualTo(fakeTime));
    }

    [Test]
    [Repeat(100)]
    public void CanBeFixed_WithDrift()
    {
        var fakeRebusTime = new FakeRebusTime();

        var fakeTime = new DateTimeOffset(2024, 3, 19, 14, 00, 00, TimeSpan.FromHours(1));

        fakeRebusTime.FakeIt(fakeTime, driftSlightly: true);

        Assert.That(fakeRebusTime.Now, Is.EqualTo(fakeTime));
        Assert.That(fakeRebusTime.Now, Is.GreaterThan(fakeTime));
        Assert.That(fakeRebusTime.Now, Is.GreaterThan(fakeTime.AddTicks(1)));
        Assert.That(fakeRebusTime.Now, Is.GreaterThan(fakeTime.AddTicks(2)));
    }

    [Test]
    public void CanBeReset()
    {
        var fakeRebusTime = new FakeRebusTime();
        var fakeTime = new DateTimeOffset(2024, 3, 19, 14, 00, 00, TimeSpan.FromHours(1));
        fakeRebusTime.FakeIt(fakeTime);

        fakeRebusTime.Reset();

        Assert.That(Math.Abs((fakeRebusTime.Now - DateTimeOffset.Now).TotalMilliseconds), Is.LessThan(10));
    }
}