using System;
using System.Collections.Generic;
using System.Text;

namespace Mall3s.Logging.Serilog.Logging
{
    /// <summary>
    /// ELK APM log
    /// </summary>
    public interface IApmLessLog : SkyApm.Logging.ILogger
    {
        void Debug(string message, object obj);
        void Error(string message, Exception exception, object obj);
        void Information(string message, object obj);
        void Trace(string message, object obj);
        void Warning(string message, object obj);
    }
}
