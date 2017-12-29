// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Serilog;

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

        public void Write(LogLevel level, FormattableString message)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    _logger.Fatal(message.Format, message.GetArguments());
                    break;
                case LogLevel.Error:
                    _logger.Error(message.Format, message.GetArguments());
                    break;
                case LogLevel.Warning:
                    _logger.Warning(message.Format, message.GetArguments());
                    break;
                case LogLevel.Information:
                    _logger.Information(message.Format, message.GetArguments());
                    break;
                case LogLevel.Verbose:
                    _logger.Verbose(message.Format, message.GetArguments());
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message.Format, message.GetArguments());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public void WriteError(Exception exception, FormattableString message)
        {
            _logger.Error(exception, message.Format, message.GetArguments());
        }
    }
}
