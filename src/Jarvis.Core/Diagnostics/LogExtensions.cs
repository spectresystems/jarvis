// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Core.Diagnostics
{
    public static class LogExtensions
    {
        public static void Fatal(this IJarvisLog log, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Fatal, null, messageTemplate, propertyValues);
        }

        public static void Fatal(this IJarvisLog log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Fatal, exception, messageTemplate, propertyValues);
        }

        public static void Error(this IJarvisLog log, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Error, null, messageTemplate, propertyValues);
        }

        public static void Error(this IJarvisLog log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Error, exception, messageTemplate, propertyValues);
        }

        public static void Warning(this IJarvisLog log, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Warning, null, messageTemplate, propertyValues);
        }

        public static void Warning(this IJarvisLog log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Warning, exception, messageTemplate, propertyValues);
        }

        public static void Information(this IJarvisLog log, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Information, null, messageTemplate, propertyValues);
        }

        public static void Information(this IJarvisLog log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Information, exception, messageTemplate, propertyValues);
        }

        public static void Debug(this IJarvisLog log, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Debug, null, messageTemplate, propertyValues);
        }

        public static void Debug(this IJarvisLog log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Debug, exception, messageTemplate, propertyValues);
        }

        public static void Verbose(this IJarvisLog log, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Debug, null, messageTemplate, propertyValues);
        }

        public static void Verbose(this IJarvisLog log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log?.Write(LogLevel.Debug, exception, messageTemplate, propertyValues);
        }
    }
}