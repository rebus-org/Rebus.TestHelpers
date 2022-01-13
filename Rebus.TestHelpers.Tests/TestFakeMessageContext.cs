using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Rebus.Extensions;
using Rebus.Messages;

namespace Rebus.TestHelpers.Tests;

[TestFixture]
public class TestFakeMessageContext
{
    [Test]
    public void CanGetCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        var token = cancellationTokenSource.Token;

        var transportMessage = new TransportMessage(new Dictionary<string, string>(), new byte[] { 1, 2, 3 });
        var context = new FakeMessageContext(transportMessage, cancellationToken: token);

        var cancellationToken = context.GetCancellationToken();

        cancellationTokenSource.Cancel();

        Assert.That(cancellationToken.IsCancellationRequested, Is.True);
    }

    [Test]
    public void CreatesFakeTransportMessage()
    {
        var context = new FakeMessageContext();

        Assert.That(context.TransportMessage, Is.Not.Null);
    }

    [Test]
    public void CanGetTransportMessage_PassInCtor()
    {
        var transportMessage = new TransportMessage(new Dictionary<string, string> { ["what"] = "iknowu" }, new byte[] { 1, 2, 3 });

        var context = new FakeMessageContext(transportMessage);

        Assert.That(context.TransportMessage.Headers["what"], Is.EqualTo("iknowu"));
        Assert.That(context.TransportMessage.Body, Is.EqualTo(new byte[] { 1, 2, 3 }));
    }

    [Test]
    public void CanGetTransportMessage_SetProperty()
    {
        var transportMessage = new TransportMessage(new Dictionary<string, string> { ["what"] = "iknowu" }, new byte[] { 1, 2, 3 });

        var context = new FakeMessageContext { TransportMessage = transportMessage };

        Assert.That(context.TransportMessage.Headers["what"], Is.EqualTo("iknowu"));
        Assert.That(context.TransportMessage.Body, Is.EqualTo(new byte[] { 1, 2, 3 }));
    }

    [Test]
    public void CanGetMessage_GetsNullByDefault()
    {
        var context = new FakeMessageContext();

        Assert.That(context.Message, Is.Null);
    }

    [Test]
    public void CanGetMessage_PassInCtor()
    {
        var headers = new Dictionary<string, string> { ["what"] = "iknowu" };
        var message = new Message(headers, new byte[] { 1, 2, 3 });

        var context = new FakeMessageContext(message: message);

        Assert.That(context.Message.Headers["what"], Is.EqualTo("iknowu"));
        Assert.That(context.Message.Body, Is.EqualTo(new byte[] { 1, 2, 3 }));
    }

    [Test]
    public void CanGetMessage_SetProperty()
    {
        var headers = new Dictionary<string, string> { ["what"] = "iknowu" };
        var message = new Message(headers, new byte[] { 1, 2, 3 });

        var context = new FakeMessageContext { Message = message };

        Assert.That(context.Message.Headers["what"], Is.EqualTo("iknowu"));
        Assert.That(context.Message.Body, Is.EqualTo(new byte[] { 1, 2, 3 }));
    }
}