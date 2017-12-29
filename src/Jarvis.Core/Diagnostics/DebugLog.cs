// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace Jarvis.Core.Diagnostics
{
    public sealed class DebugLog : IJarvisLog
    {
        public void Write(LogLevel level, FormattableString message)
        {
            Debug.WriteLine(message.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteError(Exception exception, FormattableString message)
        {
            Debug.WriteLine(message.ToString(CultureInfo.InvariantCulture));
            Debug.WriteLine(message.ToString());
        }
    }
}
