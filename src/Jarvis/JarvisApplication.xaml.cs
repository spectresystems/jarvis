// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;

namespace Jarvis
{
    public partial class JarvisApplication : IDisposable
    {
        private Mutex _mutex;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Close();
                _mutex = null;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, "29EE0B17-0217-4ACF-B58F-15AC9569C5E3");

            // Ensure only one instance is running.
            if (!_mutex.WaitOne(0, true))
            {
                MessageBox.Show("Jarvis is already running.", "Jarvis", MessageBoxButton.OK);
                Shutdown(1);
            }
            else
            {
                base.OnStartup(e);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }
    }
}
