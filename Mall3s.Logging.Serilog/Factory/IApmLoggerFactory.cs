using Mall3s.Logging.Serilog.Logging;
using SkyApm.Logging;
using SkyApm.Tracing;
using System;

namespace Mall3s.Logging.Serilog.Factory
{
    public interface IApmLoggerFactory : ILoggerFactory
    {
        IApmLessLog CreateApmLogger(Type type);

        [Obsolete("Please use method CreateApmLogger(Type type)", true)]
        new ILogger CreateLogger(Type type);
    }
}
