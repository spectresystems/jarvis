// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Core.Interop;

namespace Jarvis.Addin.Files.Drawing
{
    internal static class ShellIconLoader
    {
        private const Win32.Shgfi Flags =
            Win32.Shgfi.Icon | Win32.Shgfi.LargeIcon | Win32.Shgfi.UseFileAttributes;

        public static async Task<ImageSource> LoadImageAsync(string path, bool isDirectory)
        {
            if (path == null)
            {
                return null;
            }

            // NOTE: This is not a very good solution, but to get the correct icon
            // we need to call SHGetFileInfo from either the UI thread or from
            // a thread with an STA apartment state. We should revisit this if
            // it becomes a problem.
            var tcs = new TaskCompletionSource<ImageSource>();
            var thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(LoadImage(path, isDirectory));
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return await tcs.Task.ConfigureAwait(false);
        }

        private static ImageSource LoadImage(string path, bool isDirectory)
        {
            if (path == null)
            {
                Trace.WriteLine("Path was null.");
                return null;
            }

            var info = default(Win32.Shfileinfo);
            Win32.SHGetFileInfo(path,
                isDirectory ? Win32.FileAttribute.Directory : Win32.FileAttribute.Normal,
                out info, (uint)Marshal.SizeOf(info), Flags);

            var iconHandle = info.hIcon;
            if (iconHandle == IntPtr.Zero)
            {
                Trace.WriteLine("Could not get icon handle.");
                return null;
            }

            try
            {
                // Create the bitmap source.
                var img = Imaging.CreateBitmapSourceFromHIcon(
                    iconHandle, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                img.Freeze();
                return img;
            }
            finally
            {
                Win32.DestroyIcon(iconHandle);
            }
        }
    }
}
