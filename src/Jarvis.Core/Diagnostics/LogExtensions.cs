// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Core.Diagnostics
{
    public static class LogExtensions
    {
        public static void Fatal(this IJarvisLog log, string message)
        {
            log?.Write(LogLevel.Fatal, $"{message}");
        }

        public static void Fatal(this IJarvisLog log, FormattableString message)
        {
            log?.Write(LogLevel.Fatal, message);
        }

        public static void Error(this IJarvisLog log, string message)
        {
            log?.Write(LogLevel.Error, $"{message}");
        }

        public static void Error(this IJarvisLog log, Exception exception, string message)
        {
            log?.WriteError(exception, $"{message}");
        }

        public static void Error(this IJarvisLog log, FormattableString message)
        {
            log?.Write(LogLevel.Error, message);
        }

        public static void Error(this IJarvisLog log, Exception exception, FormattableString message)
        {
            log?.WriteError(exception, message);
        }

        public static void Warning(this IJarvisLog log, string message)
        {
            log?.Write(LogLevel.Warning, $"{message}");
        }

        public static void Warning(this IJarvisLog log, FormattableString message)
        {
            log?.Write(LogLevel.Warning, message);
        }

        public static void Information(this IJarvisLog log, string message)
        {
            log?.Write(LogLevel.Information, $"{message}");
        }

        public static void Information(this IJarvisLog log, FormattableString message)
        {
            log?.Write(LogLevel.Information, message);
        }

        public static void Debug(this IJarvisLog log, string message)
        {
            log?.Write(LogLevel.Debug, $"{message}");
        }

        public static void Debug(this IJarvisLog log, FormattableString message)
        {
            log?.Write(LogLevel.Debug, message);
        }

        public static void Verbose(this IJarvisLog log, string message)
        {
            log?.Write(LogLevel.Verbose, $"{message}");
        }

        public static void Verbose(this IJarvisLog log, FormattableString message)
        {
            log?.Write(LogLevel.Verbose, message);
        }
    }
}