using System;
using Rebus.Bus;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Represents an exception caught while dispatching a message to message handlers
    /// </summary>
    public class HandlerException
    {
        /// <summary>
        /// Gets the incoming step context for the message being handled when the exception was caught
        /// </summary>
        public IncomingStepContext IncomingStepContext { get; }

        /// <summary>
        /// Gets the caught exception
        /// </summary>
        public Exception Exception { get; }

        internal HandlerException(IncomingStepContext incomingStepContext, Exception exception)
        {
            IncomingStepContext = incomingStepContext;
            Exception = exception;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var message = IncomingStepContext.Load<Message>();

            return $@"{message.GetMessageLabel()}:

{Exception.ToString().Indented(4)}
";
        }
    }
}