// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Forms;

namespace Jarvis.Infrastructure.Utilities
{
    public static class WindowUtility
    {
        public static void CenterOnFocusedScreen(Window window)
        {
            // Get the primary screen.
            var primary = Screen.PrimaryScreen;
            if (primary == null)
            {
                return;
            }

            // Calculate the size of the monitor.
            var monitorSize = TransformDeviceToWpf(window, new Point(
                primary.WorkingArea.Right - primary.WorkingArea.Left,
                primary.WorkingArea.Bottom - primary.WorkingArea.Top));

            // Calculate the new position for the window.
            var windowX = ((monitorSize.X - window.MaxWidth) / 2) + primary.WorkingArea.Left;
            var windowY = ((monitorSize.Y - window.MaxHeight) / 2) + primary.WorkingArea.Top;

            // Set the window postion.
            window.Left = windowX;
            window.Top = windowY;
        }

        private static Point TransformDeviceToWpf(Window window, Point point)
        {
            var deviceTransform = PresentationSource.FromVisual(window)?.CompositionTarget?.TransformFromDevice;
            return deviceTransform?.Transform(point) ?? point;
        }
    }
}
