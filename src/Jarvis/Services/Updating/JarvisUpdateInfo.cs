// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Services.Updating
{
    public sealed class JarvisUpdateInfo
    {
        public Version CurrentVersion { get; set; }
        public Version FutureVersion { get; set; }
        public bool Prerelease { get; set; }
        public Uri Uri { get; set; }
    }
}