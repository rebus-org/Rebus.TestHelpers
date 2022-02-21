using System.Collections.Concurrent;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Sagas;
using Rebus.TestHelpers.Extensions;

// ReSharper disable ArgumentsStyleLiteral
#pragma warning disable CS1998

namespace Rebus.TestHelpers.Tests.Bugs;

[TestFixture]
[Description("Try to reproduce deadlock on fixture.Deliver")]
public class DoesNotDeadlockWhenDeliveringAfterAwait : FixtureBase
{
    [Test]
    public async Task YeahItWorks()
    {
        var receivedEvents = new ConcurrentQueue<string>();

        using var fixture = SagaFixture.For(() => new RandomSaga(receivedEvents));

        fixture.Deliver(new RandomMessage("hej"));
        fixture.HandlerExceptions.ThrowIfNotEmpty();

        await Task.Delay(millisecondsDelay: 17);

        fixture.Deliver(new RandomMessage("hej igen"));

        Assert.That(receivedEvents.Count, Is.EqualTo(2));
        Assert.That(receivedEvents, Is.EqualTo(new[] { "hej", "hej igen" }));
    }

    class RandomSaga : Saga<RandomSagaData>, IAmInitiatedBy<RandomMessage>
    {
        readonly ConcurrentQueue<string> _receivedEvents;

        public RandomSaga(ConcurrentQueue<string> receivedEvents) => _receivedEvents = receivedEvents;

        protected override void CorrelateMessages(ICorrelationConfig<RandomSagaData> config)
        {
            config.Correlate<RandomMessage>(m => m.CorrelationId, d => d.CorrelationId);
        }

        public async Task Handle(RandomMessage message) => _receivedEvents.Enqueue(message.CorrelationId);
    }

    record RandomMessage(string CorrelationId);

    class RandomSagaData : SagaData
    {
        public string CorrelationId { get; set; }
    }
}