using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Sagas;
using Rebus.Serialization.Json;
using JsonException = System.Text.Json.JsonException;

#pragma warning disable CS1998

namespace Rebus.TestHelpers.Tests;

[TestFixture]
public class TestSagaFixture_CustomMessageSerializer : FixtureBase
{
    [Test]
    public async Task CanSerializeMessagesWithSomethingElse()
    {
        static RebusConfigurer GetConfigurer() => Configure
            .With(new BuiltinHandlerActivator())
            .Serialization(s => s.UseNewtonsoftJson());

        using var fixture = SagaFixture.For<SomeSaga>(GetConfigurer);

        fixture.Deliver(new SomeMessageWithLoop());
    }

    [Test]
    public async Task CanSerializeMessagesWithSomethingElse_DefaultSerializer()
    {
        using var fixture = SagaFixture.For<SomeSaga>();

        var exception = Assert.Throws<JsonException>(() => fixture.Deliver(new SomeMessageWithLoop()));

        Console.WriteLine(exception);
    }

    class SomeSaga : Saga<SomeSagaData>, IHandleMessages<SomeMessageWithLoop>
    {
        protected override void CorrelateMessages(ICorrelationConfig<SomeSagaData> config)
        {
        }

        public async Task Handle(SomeMessageWithLoop message)
        {
        }
    }

    class SomeSagaData : SagaData { }

    [JsonObject(IsReference = true, ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
    class SomeMessageWithLoop
    {
        public SomeMessageWithLoopChild Child { get; }

        public SomeMessageWithLoop(SomeMessageWithLoopChild child = null)
        {
            Child = child ?? new SomeMessageWithLoopChild(this);
        }
    }

    [JsonObject(IsReference = true, ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
    class SomeMessageWithLoopChild
    {
        public SomeMessageWithLoop Parent { get; }

        public SomeMessageWithLoopChild(SomeMessageWithLoop parent)
        {
            Parent = parent;
        }
    }
}