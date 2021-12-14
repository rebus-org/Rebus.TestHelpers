using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rebus.Logging;
using Rebus.Time;

namespace Rebus.TestHelpers.Internals;

class TestLoggerFactory : AbstractRebusLoggerFactory
{
    readonly ConcurrentQueue<LogEvent> _logEvents = new();
    readonly IRebusTime _rebusTime;

    public IEnumerable<LogEvent> LogEvents => _logEvents.ToList();

    public TestLoggerFactory(IRebusTime rebusTime) => _rebusTime = rebusTime ?? throw new ArgumentNullException(nameof(rebusTime));

    protected override ILog GetLogger(Type type) => new TestLogger(type, _logEvents, this);

    class TestLogger : ILog
    {
        readonly Type _type;
        readonly ConcurrentQueue<LogEvent> _logEvents;
        readonly TestLoggerFactory _testLoggerFactory;

        public TestLogger(Type type, ConcurrentQueue<LogEvent> logEvents, TestLoggerFactory testLoggerFactory)
        {
            _type = type;
            _logEvents = logEvents;
            _testLoggerFactory = testLoggerFactory;
        }

        public void Debug(string message, params object[] objs) => Log(LogLevel.Debug, message, objs);

        public void Info(string message, params object[] objs) => Log(LogLevel.Info, message, objs);

        public void Warn(string message, params object[] objs) => Log(LogLevel.Warn, message, objs);

        public void Warn(Exception exception, string message, params object[] objs) => Log(LogLevel.Warn, message, objs, exception);

        public void Error(Exception exception, string message, params object[] objs) => Log(LogLevel.Error, message, objs, exception);

        public void Error(string message, params object[] objs) => Log(LogLevel.Error, message, objs);

        string SafeFormat(string message, object[] objs) => _testLoggerFactory.RenderString(message, objs);

        void Log(LogLevel level, string message, object[] objs, Exception exception = null)
        {
            var text = SafeFormat(message, objs);

            var logEvent = new LogEvent(
                level: level,
                text: text,
                exceptionOrNull: exception,
                sourceType: _type,
                time: _testLoggerFactory._rebusTime.Now
            );

            _logEvents.Enqueue(logEvent);
        }
    }
}