// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Serilog;
using Serilog.Context;

namespace Jarvis.Infrastructure.Diagnostics
{
    [UsedImplicitly]
    public sealed class SerilogLog : IJarvisLog
    {
        private readonly ILogger _logger;

        public SerilogLog(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope(string name, object value)
        {
            return LogContext.PushProperty(name, value);
        }

        public void Write(LogLevel logLevel, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Write(logLevel.AsSerilogLevel(), exception, messageTemplate, propertyValues);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel.AsSerilogLevel());
        }
    }
}
