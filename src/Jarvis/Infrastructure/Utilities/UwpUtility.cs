// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ---------------------------------------------------------------------
// Copyright (c) 2015 Matteo Pagani, licensed under MIT
// https://github.com/qmatteoq/DesktopBridgeHelpers
// ---------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jarvis.Infrastructure.Utilities
{
    public static class UwpUtility
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        public static bool IsRunningAsUwp()
        {
            if (IsWindows7OrLower)
            {
                return false;
            }
            else
            {
                int length = 0;
                var builder = new StringBuilder(0);
                int result = GetCurrentPackageFullName(ref length, builder);

                builder = new StringBuilder(length);
                result = GetCurrentPackageFullName(ref length, builder);

                return result != 15700L;
            }
        }

        private static bool IsWindows7OrLower
        {
            get
            {
                int versionMajor = Environment.OSVersion.Version.Major;
                int versionMinor = Environment.OSVersion.Version.Minor;
                double version = versionMajor + ((double)versionMinor / 10);
                return version <= 6.1;
            }
        }
    }
}
