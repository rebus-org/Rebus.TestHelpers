using System;
using System.Threading;

namespace Rebus.TestHelpers.Tests.Extensions;

static class ManualResetEventExtensions
{
    public static void WaitOrDie(this ManualResetEvent resetEvent, TimeSpan timeout)
    {
        if (resetEvent.WaitOne(timeout)) return;

        throw new TimeoutException($"Reset event was not set within {timeout} timeout");
    }
}