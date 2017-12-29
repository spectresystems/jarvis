// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Jarvis.Core.Interop
{
    public static partial class Win32
    {
        public enum W32SystemMetric : uint
        {
            ShuttingDown = 0x2000
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(W32SystemMetric metric);
    }
}
