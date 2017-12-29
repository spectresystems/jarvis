// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Interop;
using Win32 = Jarvis.Core.Interop.Win32;

namespace Jarvis.Infrastructure.Utilities
{
    public sealed class HotKey : IDisposable
    {
        private readonly int _id;
        private readonly Action _action;
        private bool _disposed;

        private static Dictionary<int, HotKey> _lookup;

        public HotKey(Action action)
        {
            _action = action;

            var virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key.Space);
            _id = virtualKeyCode + 65536;

            var result = Win32.RegisterHotKey(IntPtr.Zero, _id, 0x0001, (uint)virtualKeyCode);
            if (!result)
            {
                throw new InvalidOperationException("Could not register hotkey.");
            }

            if (_lookup == null)
            {
                _lookup = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += OnThreadFilterMessage;
            }
            _lookup.Add(_id, this);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Win32.UnregisterHotKey(IntPtr.Zero, _id);
                _disposed = true;
            }
        }

        private static void OnThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == Win32.WmHotKey)
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