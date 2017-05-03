using System;
using ConDep.Dsl.Logging;
using log4net;
using log4net.Appender;
using log4net.Core;

namespace ConDep.Dsl.Tests
{
    class UnitTestLogger : Log4NetLoggerBase
    {
        private readonly MemoryAppender _memAppender;

        public UnitTestLogger(ILog log4netLog, MemoryAppender memAppender) : base(log4netLog)
        {
            _memAppender = memAppender;
        }

        public override void Warn(string message, object[] formatArgs)
        {
        }

        public override void Warn(string message, Exception ex, object[] formatArgs)
        {
        }

        public override void Verbose(string message, object[] formatArgs)
        {
        }

        public override void Verbose(string message, Exception ex, object[] formatArgs)
        {
        }

        public override void Info(string message, object[] formatArgs)
        {
        }

        public override void Info(string message, Exception ex, object[] formatArgs)
        {
        }

        public override void Error(string message, object[] formatArgs)
        {
        }

        public override void Error(string message, Exception ex, object[] formatArgs)
        {
        }

        public override void Progress(string message, params object[] formatArgs)
        {
        }

        public override void ProgressEnd()
        {
        }

        public override void LogSectionStart(string name)
        {
        }

        public override void LogSectionEnd(string name)
        {
        }

        public LoggingEvent[] Events { get { return _memAppender.GetEvents(); } }

        public void ClearEvents()
        {
            _memAppender.Clear();
        }
    }
}