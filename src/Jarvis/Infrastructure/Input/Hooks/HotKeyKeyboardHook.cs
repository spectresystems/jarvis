// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Interop;
using Jarvis.Core.Interop;

namespace Jarvis.Infrastructure.Input
{
    public sealed class HotKeyKeyboardHook : IKeyboardHook
    {
        private readonly Action _action;
        private int _id;
        private bool _disposed;

        private static Dictionary<int, HotKeyKeyboardHook> _lookup;

        public HotKeyKeyboardHook(Action action)
        {
            _action = action;
        }

        public void Register()
        {
            var virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key.Space);
            _id = virtualKeyCode + 65536;

            var result = Win32.Keyboard.HotKey.RegisterHotKey(IntPtr.Zero, _id, 0x0001, (uint)virtualKeyCode);
            if (!result)
            {
                throw new InvalidOperationException("Could not register hotkey.");
            }

            if (_lookup == null)
            {
                _lookup = new Dictionary<int, HotKeyKeyboardHook>();
                ComponentDispatcher.ThreadFilterMessage += OnThreadFilterMessage;
            }

            _lookup.Add(_id, this);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Win32.Keyboard.HotKey.UnregisterHotKey(IntPtr.Zero, _id);
                _disposed = true;
            }
        }

        private static void OnThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == Win32.Keyboard.HotKey.WmHotKey)
                {
                    if (_lookup.TryGetValue((int)msg.wParam, out var hotKey))
                    {
                        hotKey._action?.Invoke();
                        handled = true;
                    }
                }
            }
        }
    }
}
