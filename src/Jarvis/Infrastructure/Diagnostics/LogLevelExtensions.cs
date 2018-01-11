using System;
using Jarvis.Core.Diagnostics;
using Serilog.Events;

namespace Jarvis.Infrastructure.Diagnostics
{
    internal static class LogLevelExtensions
    {
        public static LogEventLevel AsSerilogLevel(this LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}