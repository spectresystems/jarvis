// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

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

        public JarvisUpdateInfo ToJarvisUpdateInfo(Version currentVersion)
        {
            return new JarvisUpdateInfo
            {
                CurrentVersion = currentVersion,
                FutureVersion = Version.Parse(Name.Trim('v')),
                Uri = new Uri(HtmlUrl),
                Prerelease = Prerelease
            };
        }
    }
}
