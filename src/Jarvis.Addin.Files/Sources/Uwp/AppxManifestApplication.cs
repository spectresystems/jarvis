// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Jarvis.Addin.Files.Sources.Uwp
{
    internal sealed class AppxManifestApplication
    {
        public string Id { get; set; }
        public string AppUserModelId { get; set; }
        public string ShortName { get; set; }
        public string DisplayName { get; set; }

        public AppxManifestAsset Logo { get; set; }
        public AppxManifestAsset SmallLogo { get; set; }
        public string BackgroundColor { get; set; }

        public string EntryPoint { get; set; }
        public string Executable { get; set; }
        public string StartPage { get; set; }

        public List<AppxManifestAsset> Images { get; set; }

        public AppxManifestApplication()
        {
            Images = new List<AppxManifestAsset>();
        }
    }
}