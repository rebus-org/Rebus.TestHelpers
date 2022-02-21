using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Exceptions;
using Rebus.Sagas;
#pragma warning disable 1998

namespace Rebus.TestHelpers.Tests;


[TestFixture]
public class TestSagaFixture_CustomSerializer : FixtureBase
{



    [Test]
    public void CanRetrieveSagaData()
    {

        using var fixture = SagaFixture.For<MySaga>(() => new MicrosoftSagaSerializer());

        fixture.Deliver(new TestMessage("hej"));

        var current = fixture.Data.OfType<MySagaState>().ToList();

        Assert.That(current.Count, Is.EqualTo(1));
        Assert.That(current[0].Text, Is.EqualTo("hej"));
    }

    [Test]
    public void EmitsCreatedEvent()
    {
        using var fixture = SagaFixture.For(sagaHandlerFactory: () => new MySaga(), sagaSerializerFactory: () => new MicrosoftSagaSerializer());

        var gotEvent = false;
        fixture.Created += _ => gotEvent = true;

        fixture.Deliver(new TestMessage("hej"));

        Assert.That(gotEvent, Is.True);
    }


    class MicrosoftSagaSerializer : ISagaSerializer
    {
        public ISagaData DeserializeFromString(Type type, string str)
        {
            return (ISagaData)JsonSerializer.Deserialize(str, type);
        }

        public string SerializeToString(ISagaData obj)
        {
            var json = JsonSerializer.Serialize(obj, obj.GetType());
            return json;
        }
    }


    class MySaga : Saga<MySagaState>, IAmInitiatedBy<TestMessage>, IAmInitiatedBy<FailingTestMessage>
    {
        protected override void CorrelateMessages(ICorrelationConfig<MySagaState> config)
        {
            config.Correlate<TestMessage>(m => m.Text, d => d.Text);
            config.Correlate<FailingTestMessage>(m => m.Text, d => d.Text);
        }

        public async Task Handle(TestMessage message)
        {
            Data.Text = message.Text;

            if (message.Die) MarkAsComplete();
        }

        public Task Handle(FailingTestMessage message)
        {
            throw new RebusApplicationException("oh no something bad happened");
        }
    }

    class TestMessage
    {
        public TestMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
        public bool Die { get; set; }
    }

    class FailingTestMessage
    {
        public FailingTestMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }

    class MySagaState : ISagaData
    {
        public Guid Id { get; set; }
        public int Revision { get; set; }
        public string Text { get; set; }
    }
}