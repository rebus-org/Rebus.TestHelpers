using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Sagas;

namespace Rebus.TestHelpers.Tests.Bugs;

[TestFixture]
public class TestAsyncConcurrencyChallengeWithSagaFixture
{
    [Test]
    public async Task SeeIfItWorksWhenSettingMaxDeliveryAttempts()
    {
        using var fixture = SagaFixture.For(() => new MySaga(), maxDeliveryAttempts: 1);

        fixture.Deliver(new MyMessage());

        // wait a while to allow for any stuff to settle
        await Task.Delay(TimeSpan.FromSeconds(Random.Shared.NextDouble()));

        var exceptions = fixture.HandlerExceptions.ToList();

        Assert.That(exceptions.Count, Is.EqualTo(1), 
            "Expected exactly one single exception, because the MAX number of delivery attempts was set to 1");
    }

    class MyMessage { }

    class MySagaData : SagaData { }

    class MySaga : Saga<MySagaData>, IAmInitiatedBy<MyMessage>
    {
        protected override void CorrelateMessages(ICorrelationConfig<MySagaData> config)
        {
            throw new System.NotImplementedException();
        }

        public Task Handle(MyMessage message) => throw new AbandonedMutexException("oh no");
    }
}