using System;
using Rebus.Logging;
using Rebus.Time;

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Represents a log event emitted from Rebus' internals during saga testing with <see cref="SagaFixture{TSagaHandler}"/>
    /// </summary>
    public class LogEvent
    {
        internal LogEvent(LogLevel level, string text, Exception exceptionOrNull, Type sourceType, DateTimeOffset time)
        {
            Time = time;
            Level = level;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            ExceptionOrNull = exceptionOrNull;
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        }

        /// <summary>
        /// Gets the (Rebus) time of when the event was emitted
        /// </summary>
        public DateTimeOffset Time { get; }
        
        /// <summary>
        /// Gets the associated log level
        /// </summary>
        public LogLevel Level { get; }
        
        /// <summary>
        /// Gets a string representation of the log event
        /// </summary>
        public string Text { get; }
        
        /// <summary>
        /// Gets the associated exception (or null if none was included)
        /// </summary>
        public Exception ExceptionOrNull { get; }

        /// <summary>
        /// Gets the type that the logger was associated with
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// Gets a string-formatted version of the log event
        /// </summary>
        public override string ToString()
        {
            var exceptionText = ExceptionOrNull == null ? "" : $" - {ExceptionOrNull}";

            return $"{Time:HH:mm:ss} [{Level}] [{SourceType.Name}] {Text}{exceptionText}";
        }
    }
}