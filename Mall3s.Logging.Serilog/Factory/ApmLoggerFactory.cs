using System;
using ILogger = SkyApm.Logging.ILogger;
using MSLoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;
using Microsoft.Extensions.Logging;
using SkyApm.Tracing;
using Mall3s.Logging.Serilog.Logging;

namespace Mall3s.Logging.Serilog.Factory
{
    public class ApmLoggerFactory : IApmLoggerFactory
    {
        private readonly MSLoggerFactory _loggerFactory;
        private readonly IEntrySegmentContextAccessor _context;
        public ApmLoggerFactory (IEntrySegmentContextAccessor context)
        {
            _loggerFactory = new MSLoggerFactory();
            _context = context;
        }

        public IApmLessLog CreateApmLogger(Type type)
        {
            return new ApmLessLog(_loggerFactory.CreateLogger(type), _context, type);
        }

        public ILogger CreateLogger(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
