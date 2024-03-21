using System;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;

namespace Rebus.TestHelpers;

/// <summary>
/// Scope that establishes a fake message context, enabling that <see cref="MessageContext.Current"/> will return a <see cref="IMessageContext"/>
/// </summary>
public class FakeMessageContextScope : IDisposable
{
    readonly RebusTransactionScope _scope;

    /// <summary>
    /// Creates the fake message context as if <paramref name="transportMessage"/> is the received <see cref="TransportMessage"/>.
    /// </summary>
    public FakeMessageContextScope(TransportMessage transportMessage)
    {
        if (transportMessage == null) throw new ArgumentNullException(nameof(transportMessage));

        _scope = new RebusTransactionScope();

        var transactionContext = _scope.TransactionContext;

        transactionContext.Items[StepContext.StepContextKey] =
            new IncomingStepContext(transportMessage, transactionContext);
    }

    public void Dispose() => _scope.Dispose();
}