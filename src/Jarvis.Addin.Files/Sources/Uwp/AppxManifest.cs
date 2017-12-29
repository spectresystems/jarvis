// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Sources.Uwp
{
    internal sealed class AppxManifest
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public AppxManifestAsset Logo { get; set; }
        public string Publisher { get; set; }

        public List<AppxManifestApplication> Applications { get; }

        public AppxManifest()
        {
            Applications = new List<AppxManifestApplication>();
        }
    }

    internal sealed class AppxManifestAsset
    {
        public string Name { get; set; }
        public FilePath Path { get; set; }
    }
}
