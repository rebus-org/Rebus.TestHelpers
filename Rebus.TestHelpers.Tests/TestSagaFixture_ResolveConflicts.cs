using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Sagas;
using Rebus.TestHelpers.Tests.Extensions;
// ReSharper disable UnusedAutoPropertyAccessor.Local

#pragma warning disable 1998

namespace Rebus.TestHelpers.Tests;

[TestFixture]
public class TestSagaFixture_ResolveConflicts : FixtureBase
{
    [Test]
    public void CanSimulateConflict()
    {
        using var fixture = SagaFixture.For(() => new ConflictSagaHandler());

        fixture.DumpLogsOnDispose();

        fixture.Deliver(new SomeMessage("some-id", "HEJ MED DIG"));

        var sagaDataId = fixture.Data.OfType<ConflictSagaHandlerData>().FirstOrDefault(s => s.CorrelationId == "some-id")?.Id
                   ?? throw new ArgumentException("Could not find saga with correlation ID 'some-id'");

        fixture.PrepareConflict<ConflictSagaHandlerData>(sagaDataId, data =>
        {
            data.ReceivedStrings.Add("DIN SVESKE");
        });

        fixture.Deliver(new SomeMessage("some-id", "MIN VEN"));

        var sagaData = fixture.Data
                           .OfType<ConflictSagaHandlerData>()
                           .FirstOrDefault(d => d.CorrelationId == "some-id")
                       ?? throw new AssertionException("Could not find saga data with correlation ID 'some-id'");

        string GetReceivedStrings() =>
            $@"Got these strings:

{string.Join(Environment.NewLine, sagaData.ReceivedStrings)}
";

        Assert.That(sagaData.ReceivedStrings.Count, Is.EqualTo(3), GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("HEJ MED DIG"), Is.True, GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("MIN VEN"), Is.True, GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("DIN SVESKE"), Is.True, GetReceivedStrings);
    }

    [Test]
    public void CanSimulateConflict_MultipleConflicts()
    {
        using var fixture = SagaFixture.For(() => new ConflictSagaHandler());

        fixture.DumpLogsOnDispose();

        fixture.Deliver(new SomeMessage("some-id", "HEJ MED DIG"));

        fixture.PrepareConflict<ConflictSagaHandlerData>(d => d.CorrelationId == "some-id", data => data.ReceivedStrings.Add("CONFLICT1"));

        fixture.Deliver(new SomeMessage("some-id", "MIN VEN"));

        fixture.PrepareConflict<ConflictSagaHandlerData>(d => d.CorrelationId == "some-id", data => data.ReceivedStrings.Add("CONFLICT2"));

        fixture.Deliver(new SomeMessage("some-id", "IGEN"));

        var sagaData = fixture.Data
                           .OfType<ConflictSagaHandlerData>()
                           .FirstOrDefault(d => d.CorrelationId == "some-id")
                       ?? throw new AssertionException("Could not find saga data with correlation ID 'some-id'");

        string GetReceivedStrings() =>
            $@"Got these strings:

    {string.Join(Environment.NewLine, sagaData.ReceivedStrings)}
    ";

        Assert.That(sagaData.ReceivedStrings.Count, Is.EqualTo(5), GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("HEJ MED DIG"), Is.True, GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("MIN VEN"), Is.True, GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("IGEN"), Is.True, GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("CONFLICT1"), Is.True, GetReceivedStrings);
        Assert.That(sagaData.ReceivedStrings.Contains("CONFLICT2"), Is.True, GetReceivedStrings);
    }

    class SomeMessage
    {
        public SomeMessage(string correlationId, string text)
        {
            CorrelationId = correlationId;
            Text = text;
        }

        public string CorrelationId { get; }
        public string Text { get; }
    }

    class ConflictSagaHandler : Saga<ConflictSagaHandlerData>, IAmInitiatedBy<SomeMessage>
    {
        protected override void CorrelateMessages(ICorrelationConfig<ConflictSagaHandlerData> config)
        {
            config.Correlate<SomeMessage>(m => m.CorrelationId, d => d.CorrelationId);
        }

        protected override async Task ResolveConflict(ConflictSagaHandlerData otherSagaData)
        {
            var stringsMissingFromThisSagaData = otherSagaData.ReceivedStrings.Except(Data.ReceivedStrings).ToList();

            Data.ReceivedStrings.AddRange(stringsMissingFromThisSagaData);
        }

        public async Task Handle(SomeMessage message)
        {
            Data.ReceivedStrings.Add(message.Text);
        }
    }

    class ConflictSagaHandlerData : SagaData
    {
        public List<string> ReceivedStrings { get; } = new();
        public string CorrelationId { get; set; }
    }
}