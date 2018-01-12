// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Core.Diagnostics
{
    public interface IJarvisLog
    {
        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="name"> Name of the scope.</param>
        /// <param name="value"> Value of the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        IDisposable BeginScope(string name, object value);

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="messageTemplate">Message template associated with log entry</param>
        /// <param name="propertyValues">Property values associated with the message template</param>
        void Write(LogLevel logLevel, Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Checks if a certain log level is enabled for the instance of the logger.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>true if the log level is enabled, otherwise false.</returns>
        bool IsEnabled(LogLevel logLevel);
    }
}
