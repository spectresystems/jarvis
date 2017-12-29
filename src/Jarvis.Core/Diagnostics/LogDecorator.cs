// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace Jarvis.Core.Diagnostics
{
    public sealed class LogDecorator : IJarvisLog
    {
        private readonly string _name;
        private readonly IJarvisLog _log;

        public LogDecorator(string name, IJarvisLog log)
        {
            _name = name;
            _log = log;
        }

        public void Write(LogLevel level, FormattableString message)
        {
            _log.Write(level, $"[{_name}] {message.ToString(CultureInfo.InvariantCulture)}");
        }

        public void WriteError(Exception exception, FormattableString message)
        {
            _log.WriteError(exception, $"[{_name}] {message.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}
