// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Spectre.System.IO;
using Windows.ApplicationModel;

// ReSharper disable once CheckNamespace
namespace Jarvis.Addin.Files
{
    internal static class PackageExtensions
    {
        public static bool TryGetInstallLocation(this Package package, out DirectoryPath path)
        {
            try
            {
                path = new DirectoryPath(package.InstalledLocation.Path);
                return true;
            }
            catch (Exception)
            {
                path = null;
                return false;
            }
        }
    }
}
