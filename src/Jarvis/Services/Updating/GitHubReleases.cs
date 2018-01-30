// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Semver;

namespace Jarvis.Services.Updating
{
    public class GitHubRelease
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }

        [JsonProperty("draft")]
        public bool Draft { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("assets")]
        public List<GitHubReleaseAsset> Assets { get; set; }

        public JarvisUpdateInfo ToJarvisUpdateInfo(SemVersion currentVersion)
        {
            var installer = Assets.FirstOrDefault(x => x.DownloadUrl?.EndsWith(".exe") ?? false);

            return new JarvisUpdateInfo
            {
                CurrentVersion = currentVersion,
                FutureVersion = SemVersion.Parse(Name.Trim('v')),
                ReleaseUri = new Uri(HtmlUrl),
                Prerelease = Prerelease,
                InstallerName = installer?.FileName,
                InstallerUri = installer?.DownloadUrl != null ? new Uri(installer.DownloadUrl) : null
            };
        }
    }
}
