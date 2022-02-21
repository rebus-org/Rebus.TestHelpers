using System;
using System.Threading;
using System.Threading.Tasks;
using Rebus.Pipeline;
using Rebus.Transport;
// ReSharper disable ArgumentsStyleLiteral

namespace Rebus.TestHelpers.Internals;

/// <summary>
/// Pipeline step that can be used to synchronize the processing of messages. A semaphore is used internally, which
/// is incremented each time a message has been fully processed (either successfully or with error). The <see cref="WaitOne"/>
/// can then be used to decrement the semaphore, thus blocking until a message has been processed.
/// </summary>
[StepDocumentation("Step that can be used to synchronize message processing")]
class LockStepper : IIncomingStep, IDisposable
{
    readonly Semaphore _semaphore = new(initialCount: 0, maximumCount: int.MaxValue);

    public async Task Process(IncomingStepContext context, Func<Task> next)
    {
        context.Load<ITransactionContext>()
            .OnDisposed(_ => _semaphore.Release());

        await next();
    }

    public bool WaitOne(TimeSpan timeout) => _semaphore.WaitOne(timeout: timeout);

    public void Dispose() => _semaphore?.Dispose();
}