using System;
using Rebus.Sagas;

namespace Rebus.TestHelpers.Tests.Extensions;

static class SagaFixtureExtensions
{
    public static void DumpLogsOnDispose<TSagaHandler>(this SagaFixture<TSagaHandler> sagaFixture)
        where TSagaHandler : Saga
    {
        sagaFixture.Disposed += () =>
        {
            Console.WriteLine(string.Join(Environment.NewLine, sagaFixture.LogEvents));
        };
    }
}