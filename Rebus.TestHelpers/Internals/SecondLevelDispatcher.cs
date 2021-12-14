using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Retry;

namespace Rebus.TestHelpers.Internals;

[StepDocumentation("Step that transforms a message by wrapping it in IFailed, dispatching it to the rest of the pipeline")]
class SecondLevelDispatcher : IIncomingStep
{
    public const string SecondLevelDispatchExceptionId = "2nd-level-dispatcher";

    readonly ConcurrentDictionary<string, Exception> _exceptions = new ConcurrentDictionary<string, Exception>();
    readonly IErrorTracker _errorTracker;

    public SecondLevelDispatcher(IErrorTracker errorTracker)
    {
        _errorTracker = errorTracker ?? throw new ArgumentNullException(nameof(errorTracker));
    }

    public async Task Process(IncomingStepContext context, Func<Task> next)
    {
        var transportMessage = context.Load<TransportMessage>();
        var headers = transportMessage.Headers;

        if (headers.TryGetValue(SecondLevelDispatchExceptionId, out var id))
        {
            if (!_exceptions.TryRemove(id, out var exception))
            {
                throw new ArgumentException($"Could not find exception with ID {id}");
            }

            var messageId = headers.GetValue(Headers.MessageId);

            _errorTracker.RegisterError(messageId, exception);
            _errorTracker.MarkAsFinal(messageId);
        }

        await next();
    }

    public string PrepareException(Exception exception)
    {
        var id = Guid.NewGuid().ToString();
        _exceptions[id] = exception;
        return id;
    }
}