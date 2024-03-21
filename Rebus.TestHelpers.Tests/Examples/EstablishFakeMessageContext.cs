using System;
using NUnit.Framework;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;

namespace Rebus.TestHelpers.Tests.Examples;

[TestFixture]
public class EstablishFakeMessageContext : FixtureBase
{
    [Test]
    public void ThisIsWhatWeWantToDo()
    {
        // create ambient transaction context
        using var scope = new RebusTransactionScope();

        scope.TransactionContext.Items[StepContext.StepContextKey] =
            new IncomingStepContext(new TransportMessage(new(), Array.Empty<byte>()), scope.TransactionContext);

        var messageContext = MessageContext.Current;

        Assert.That(messageContext, Is.Not.Null);
    }

    [Test]
    public void ThisIsHowItIsDone_MessageContextIsThere()
    {
        var transportMessage = new TransportMessage(new(), Array.Empty<byte>());

        using var scope = new FakeMessageContextScope(transportMessage);

        var messageContext = MessageContext.Current;

        Assert.That(messageContext.TransportMessage, Is.SameAs(transportMessage));
    }

    [Test]
    public void ThisIsHowItIsDone_MessageContextGetsClearedAfterExit()
    {
        var transportMessage = new TransportMessage(new(), Array.Empty<byte>());

        using (new FakeMessageContextScope(transportMessage))
        {
            Assert.That(MessageContext.Current, Is.Not.Null);
        }

        Assert.That(MessageContext.Current, Is.Null);
    }
}