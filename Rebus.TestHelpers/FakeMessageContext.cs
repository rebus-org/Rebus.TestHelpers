using System;
using System.Collections.Generic;
using System.Threading;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
// ReSharper disable UnusedMember.Global

namespace Rebus.TestHelpers;

/// <summary>
/// Implementation of <see cref="IMessageContext"/> that can be used when unit testing handlers.
/// </summary>
public class FakeMessageContext : IMessageContext
{
    readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Creates the fake message context with the specified <paramref name="transportMessage"/>.
    /// If either <paramref name="message"/> or <paramref name="cancellationToken"/> are passed in as well, they will be added to the message context.
    /// </summary>
    public FakeMessageContext(TransportMessage transportMessage = null, Message message = null, CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
        SetIncomingStepContext(transportMessage ?? CreateFakeTransportMessage());
        Message = message;
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/>, because the fake message context does not have a <see cref="ITransactionContext"/>. If you need a transaction context,
    /// you'll probably want to configure an in-mem bus and have it dispatch a message to your handler, because it's pretty tightly integrated with how Rebus works, and so
    /// it would be more true to how it works by using a full bus.
    /// </summary>
    public ITransactionContext TransactionContext => throw new InvalidOperationException("The fake message context does not have a valid transaction context, sorry");

    /// <summary>
    /// Gets the incoming step context
    /// </summary>
    public IncomingStepContext IncomingStepContext { get; private set; }

    /// <summary>
    /// Gets the transport message if one was passed to the constructor or throws an exception. In the real world, a transport message will always be present in a message context.
    /// </summary>
    public TransportMessage TransportMessage
    {
        get => IncomingStepContext.Load<TransportMessage>();
        set => SetIncomingStepContext(value);
    }

    /// <summary>
    /// Gets the logical message if one was passed to the constructor, null otherwise. The <see cref="Message"/> will be stored in the current <see cref="IncomingStepContext"/>
    /// </summary>
    public Message Message
    {
        get => IncomingStepContext.Load<Message>();
        set => IncomingStepContext.Save(value);
    }

    /// <summary>
    /// Gets the message headers from the transport message
    /// </summary>
    public Dictionary<string, string> Headers => TransportMessage.Headers;

    void SetIncomingStepContext(TransportMessage transportMessage)
    {
        IncomingStepContext = new IncomingStepContext(transportMessage, new TransactionContext());
        IncomingStepContext.Save(_cancellationToken);
    }

    static TransportMessage CreateFakeTransportMessage()
    {
        var headers = new Dictionary<string, string>
        {
            [Messages.Headers.MessageId] = $"fake-{Guid.NewGuid():D}",
            ["Description"] = "This is a fake transport message with an empty body created for this fake message context. It can be overridden by passing a TransportMessage to the FakeMessageContext ctor, or by using its TransportMessage set property."
        };
        return new TransportMessage(headers, Array.Empty<byte>());
    }
}