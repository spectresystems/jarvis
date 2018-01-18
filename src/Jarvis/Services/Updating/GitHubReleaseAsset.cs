// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Jarvis.Services.Updating
{
    public class GitHubReleaseAsset
    {
        [JsonProperty("name")]
        public string FileName { get; set; }

        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}