// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Jarvis.Core.Interop
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static partial class Win32
    {
        public static class Keyboard
        {
            public delegate IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam);

#pragma warning disable SA1310 // Field names must not contain underscore
            public const int WH_KEYBOARD_LL = 13;
            public const int WM_KEYDOWN = 256;
            public const int WM_SYSKEYDOWN = 260;
#pragma warning restore SA1310 // Field names must not contain underscore

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, HookCallback lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        }
    }
}
