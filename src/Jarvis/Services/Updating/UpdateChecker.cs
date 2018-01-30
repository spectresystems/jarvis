// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Semver;

namespace Jarvis.Services.Updating
{
    [UsedImplicitly]
    public sealed class UpdateChecker : IUpdateChecker
    {
        private readonly ISettingsStore _settings;
        private readonly IJarvisLog _log;
        private readonly SemVersion _currentVersion;
        private readonly HttpClient _client;

        public UpdateChecker(ISettingsStore settings, IJarvisLog log)
        {
            _settings = settings;
            _log = log;
            _currentVersion = new SemVersion(typeof(UpdateService).Assembly.GetName().Version);

            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("Jarvis", _currentVersion.ToString()));
        }

        public async Task<JarvisUpdateInfo> CheckForUpdates()
        {
            try
            {
                _log.Verbose("Getting releases from GitHub.");
                var uri = new Uri("https://api.github.com/repos/spectresystems/jarvis/releases");
                var response = await _client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    _log.Warning($"GitHub returned status code {response.StatusCode} ({(int)response.StatusCode}) when checking for updates.");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();

                // Get the last release.
                var result = JsonConvert.DeserializeObject<List<GitHubRelease>>(json)
                    .Where(x => !x.Draft);

                // Should we not include previews?
                if (!_settings.Get<bool>(Constants.Settings.General.IncludePreviews))
                {
                    result = result.Where(x => !x.Prerelease);
                }

                // Return the latest version.
                return result.OrderByDescending(x => x.PublishedAt)
                    .FirstOrDefault()?.ToJarvisUpdateInfo(_currentVersion);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "An exception occured while checking for updates at GitHub.");
                return null;
            }
        }
    }
}