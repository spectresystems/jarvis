// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Semver;

namespace Jarvis.Services.Updating
{
    public sealed class JarvisUpdateInfo
    {
        public SemVersion CurrentVersion { get; set; }
        public SemVersion FutureVersion { get; set; }
        public bool Prerelease { get; set; }
        public Uri ReleaseUri { get; set; }
        public string InstallerName { get; set; }
        public Uri InstallerUri { get; set; }
    }
}