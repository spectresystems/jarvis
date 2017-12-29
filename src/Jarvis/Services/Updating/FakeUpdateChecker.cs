// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Jarvis.Services.Updating
{
    public abstract class FakeUpdateChecker : IUpdateChecker
    {
        private readonly Version _currentVersion;
        private readonly Version _futureVersion;
        private readonly TimeSpan _delay;

        [UsedImplicitly]
        public sealed class WithUpdate : FakeUpdateChecker
        {
            public WithUpdate()
                : base(new Version(0, 0, 1, 0), new Version(99, 99, 99, 99), TimeSpan.FromSeconds(10))
            {
            }
        }

        [UsedImplicitly]
        public sealed class WithoutUpdate : FakeUpdateChecker
        {
            public WithoutUpdate()
                : base(new Version(0, 0, 1, 0), new Version(0, 0, 1, 0), TimeSpan.Zero)
            {
            }
        }

        private FakeUpdateChecker(Version currentVersion, Version futureVersion, TimeSpan delay)
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
                Uri = new Uri("https://github.com/cake-build/cake/releases/tag/v0.23.0")
            };
        }
    }
}
