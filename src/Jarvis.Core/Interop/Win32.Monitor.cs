// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Jarvis.Core.Interop
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static partial class Win32
    {
        public static class Monitor
        {
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromPoint(W32Point pt, uint dwFlags);

            [StructLayout(LayoutKind.Sequential)]
            [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
            public struct MonitorInfo
            {
                public int Size;
                public W32Rect Monitor;
                public W32Rect WorkArea;
                public uint Flags;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public char[] Name;
            }
        }
    }
}
