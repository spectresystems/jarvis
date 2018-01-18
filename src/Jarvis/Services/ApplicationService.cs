// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Caliburn.Micro;
using Jarvis.Infrastructure.Utilities;
using JetBrains.Annotations;
using Win32 = Jarvis.Core.Interop.Win32;

namespace Jarvis.Services
{
    [UsedImplicitly]
    public sealed class ApplicationService
    {
        private readonly JarvisApplication _application;

        public ApplicationService(
            JarvisApplication application,
            IEventAggregator inbox)
        {
            _application = application;
        }

        public bool IsWindowsShuttingDown()
        {
            return Win32.System.GetSystemMetrics(Win32.System.W32SystemMetric.ShuttingDown) != 0;
        }

        public void Quit()
        {
            Show();
            _application.MainWindow?.Close();
        }

        public void Toggle()
        {
            if (_application?.MainWindow != null)
            {
                if (_application.MainWindow.Visibility == Visibility.Hidden)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        public void Show()
        {
            if (_application?.MainWindow != null)
            {
                if (_application.MainWindow.Visibility != Visibility.Visible)
                {
                    WindowUtility.CenterOnFocusedScreen(_application.MainWindow);

                    _application.MainWindow.Topmost = true;
                    _application.MainWindow.Show();
                    _application.MainWindow.Activate();
                    _application.MainWindow.Focus();
                }
            }
        }

        public void Hide()
        {
            if (_application?.MainWindow != null)
            {
                if (_application.MainWindow.Visibility != Visibility.Hidden &&
                    _application.MainWindow.Visibility != Visibility.Collapsed)
                {
                    _application.MainWindow.Topmost = false;
                    _application.MainWindow.Hide();
                }
            }
        }
    }
}
