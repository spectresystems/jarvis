// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Jarvis.Core.Interop
{
    public static partial class Win32
    {
        public static class Window
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hwnd, ref W32Rect rectangle);
        }
    }
}
