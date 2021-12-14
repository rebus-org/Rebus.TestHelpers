using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Exceptions;
using Rebus.Logging;
using Rebus.Sagas;
using Rebus.TestHelpers.Tests.Extensions;

namespace Rebus.TestHelpers.Tests;

[TestFixture]
public class TestSagaFixture_FailFast : FixtureBase
{
    [Test]
    [Description("When 2nd level retries was always enabled, throwing an error would result in two WARN level logs: One for the error, and one for not finding a handler for IFailed<TheMessage>")]
    public void Reproduce()
    {
        using var fixture = SagaFixture.For(() => new MySaga());

        fixture.DumpLogsOnDispose();

        fixture.Deliver(new MyMessage());

        Task.Delay(TimeSpan.FromSeconds(1)).Wait();

        var logs = fixture.LogEvents.ToList();

        Assert.That(logs.Count(l => l.Level == LogLevel.Warn), Is.EqualTo(1));
    }

    record MyMessage();

    class MyFailFastException : Exception, IFailFastException { }

    class MySaga : Saga<MySagaData>, IAmInitiatedBy<MyMessage>
    {
        protected override void CorrelateMessages(ICorrelationConfig<MySagaData> config)
        {
            config.Correlate<MyMessage>(_ => Guid.NewGuid(), d => d.Id);
        }

        public Task Handle(MyMessage message) => throw new MyFailFastException();
    }

    class MySagaData : SagaData
    {
    }
}