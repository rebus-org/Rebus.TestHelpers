using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Pipeline;

namespace Rebus.TestHelpers.Internals;

[StepDocumentation("Step that can be used to collect information about caught exceptions")]
class ExceptionCollector : IIncomingStep
{
    readonly List<HandlerException> _caughtExceptions = new List<HandlerException>();

    public IEnumerable<HandlerException> CaughtExceptions => _caughtExceptions;

    public async Task Process(IncomingStepContext context, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch(Exception exception)
        {
            _caughtExceptions.Add(new HandlerException(context, exception));

            throw;
        }
    }
}