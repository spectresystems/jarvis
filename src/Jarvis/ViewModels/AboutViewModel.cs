// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace Jarvis.ViewModels
{
    public sealed class AboutViewModel : Screen
    {
        public delegate AboutViewModel Factory();

        [UsedImplicitly]
        public Uri Licenses { get; }
        [UsedImplicitly]
        public string Version { get; }
        [UsedImplicitly]
        public DateTime BuildDate { get; }

        public AboutViewModel()
        {
            Licenses = new Uri("pack://application:,,,/Jarvis;component/Resources/Licenses.rtf");
            BuildDate = GetLinkerTime();

            var info = FileVersionInfo.GetVersionInfo(typeof(AboutViewModel).Assembly.Location);
            Version = info.ProductVersion;
        }

        [UsedImplicitly]
        public void CloseAbout()
        {
            TryClose(true);
        }

        // https://stackoverflow.com/a/1600990/936
        private static DateTime GetLinkerTime()
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            var buffer = new byte[2048];

            var filePath = typeof(AboutViewModel).Assembly.Location;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                stream.Read(buffer, 0, 2048);
            }

            var offset = BitConverter.ToInt32(buffer, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + linkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            return TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, TimeZoneInfo.Local);
        }
    }
}
