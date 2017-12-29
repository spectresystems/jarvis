// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Addin.Files.Sources.Uwp
{
    internal static class AppxPackagingInteropExtensions
    {
        public static string ReadString(this AppxPackagingInterop.IAppxManifestProperties properties, string name)
        {
            try
            {
                return properties.GetStringValue(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ReadString(this AppxPackagingInterop.IAppxManifestApplication application, string name)
        {
            try
            {
                return application.GetStringValue(name);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
