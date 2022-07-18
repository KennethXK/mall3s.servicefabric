
using Microsoft.Extensions.Logging;
using System;
using SkyApm.Tracing;
using Mall3s.Logging.Serilog.Logging;

namespace Mall3s.Logging.Serilog
{
    /// <summary>
    /// APM的Log
    /// </summary>
    public class ApmLessLog : IApmLessLog
    {
        private readonly ILogger _readLogger;
        private readonly IEntrySegmentContextAccessor _contextAccessor;
        private readonly Type _type;

        public ApmLessLog(ILogger readLogger, IEntrySegmentContextAccessor contextAccessor,Type type)
        {
            _readLogger = readLogger;
            _contextAccessor = contextAccessor;
            _type = type;
        }

        public void Debug(string message)
        {
            _readLogger.LogDebug(message);
        }

        public void Debug(string message, object obj)
        {
            _readLogger.LogDebug(message);
        }

        public void Error(string message, Exception exception)
        {
            _readLogger.LogError(message + Environment.NewLine + exception);
        }

        public void Error(string message, Exception exception, object obj)
        {
            _readLogger.LogError(message + Environment.NewLine + exception);
        }

        public void Information(string message)
        {
            _readLogger.LogInformation(message);
        }

        public void Information(string message, object obj)
        {
            _readLogger.LogInformation(message);
        }

        public void Trace(string message)
        {
            _readLogger.LogTrace(message);
        }

        public void Trace(string message, object obj)
        {
            _readLogger.LogTrace(message);
        }

        public void Warning(string message)
        {
            _readLogger.LogWarning(message);
        }

        public void Warning(string message, object obj)
        {
            _readLogger.LogWarning(message);
        }

        
    }
}
