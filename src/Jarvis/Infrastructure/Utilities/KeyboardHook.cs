// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Jarvis.Core.Interop;

namespace Jarvis.Infrastructure.Utilities
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public sealed class KeyboardHook : CriticalFinalizerObject, IDisposable
    {
        private readonly Action _action;
        private readonly IntPtr _hook;
        private readonly Win32.Keyboard.HookCallback _callback;
        private bool _disposed;

        public KeyboardHook(Action action)
        {
            _action = action;
            _callback = Callback;

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hook = Win32.Keyboard.SetWindowsHookEx(
                    Win32.Keyboard.WH_KEYBOARD_LL,
                    _callback,
                    Win32.Process.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        ~KeyboardHook()
        {
            Dispose();
        }

        private IntPtr Callback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)Win32.Keyboard.WM_KEYDOWN ||
                    wParam == (IntPtr)Win32.Keyboard.WM_SYSKEYDOWN)
                {
                    var vkCode = Marshal.ReadInt32(lParam);
                    var keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
                    Trace.WriteLine(keyPressed);
                    if (Keyboard.Modifiers == ModifierKeys.Alt && keyPressed == Key.Space)
                    {
                        _action();

                        // We've handled the hook.
                        return (IntPtr)1;
                    }
                }
            }
            return Win32.Keyboard.CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_hook != IntPtr.Zero)
                {
                    Win32.Keyboard.UnhookWindowsHookEx(_hook);
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
