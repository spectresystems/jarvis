// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Semver;

namespace Jarvis.Services.Updating
{
    public abstract class FakeUpdateChecker : IUpdateChecker
    {
        private readonly SemVersion _currentVersion;
        private readonly SemVersion _futureVersion;
        private readonly TimeSpan _delay;

        [UsedImplicitly]
        public sealed class WithUpdate : FakeUpdateChecker
        {
            public WithUpdate()
                : base(new SemVersion(0, 0, 1), new SemVersion(99, 99, 99), TimeSpan.FromSeconds(10))
            {
            }
        }

        [UsedImplicitly]
        public sealed class WithoutUpdate : FakeUpdateChecker
        {
            public WithoutUpdate()
                : base(new SemVersion(0, 0, 1), new SemVersion(0, 0, 1), TimeSpan.Zero)
            {
            }
        }

        private FakeUpdateChecker(SemVersion currentVersion, SemVersion futureVersion, TimeSpan delay)
        {
            _currentVersion = currentVersion;
            _futureVersion = futureVersion;
            _delay = delay;
        }

        public async Task<JarvisUpdateInfo> CheckForUpdates()
        {
            await Task.Delay(_delay);

            return new JarvisUpdateInfo
            {
                CurrentVersion = _currentVersion,
                FutureVersion = _futureVersion,
                Prerelease = false,
                ReleaseUri = new Uri("https://github.com/spectresystems/jarvis/releases/tag/v0.6.0"),
                InstallerName = "Jarvis-0.6.0-x64.exe",
                InstallerUri = new Uri("https://github.com/spectresystems/jarvis/releases/download/v0.6.0/Jarvis-0.6.0-x64.exe")
            };
        }
    }
}
