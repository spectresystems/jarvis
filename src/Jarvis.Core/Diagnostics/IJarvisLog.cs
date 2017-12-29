// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Core.Diagnostics
{
    public interface IJarvisLog
    {
        void Write(LogLevel level, FormattableString message);
        void WriteError(Exception exception, FormattableString message);
    }
}
