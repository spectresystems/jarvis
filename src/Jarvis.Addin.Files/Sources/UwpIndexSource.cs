// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Jarvis.Addin.Files.Extensions;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Addin.Files.Sources.Uwp;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Spectre.System.IO;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace Jarvis.Addin.Files.Sources
{
    [UsedImplicitly]
    internal sealed class UwpIndexSource : IFileIndexSource
    {
        private readonly AppxManifestReader _reader;
        private readonly IFileSystem _fileSystem;
        private readonly IJarvisLog _log;

        public string Name => "UWP";

        public UwpIndexSource(AppxManifestReader reader, IFileSystem fileSystem, IJarvisLog log)
        {
            _reader = reader;
            _fileSystem = fileSystem;
            _log = log;
        }

        public IEnumerable<IndexedEntry> Index()
        {
            var user = WindowsIdentity.GetCurrent().User?.Value;
            if (user == null)
            {
                _log.Error("Could not retrieve current user.");
                return Enumerable.Empty<IndexedEntry>();
            }

            // Get packages.
            var manager = new PackageManager();
            var packages = manager.FindPackagesForUser(user);

            var result = new List<IndexedEntry>();
            foreach (var package in packages.Where(p => !p.IsDevelopmentMode && !p.IsFramework && !p.IsResourcePackage && !p.IsBundle))
            {
                if (TryGetIndexedEntry(package, out var entries))
                {
                    entries.ForEach(entry => result.Add(entry));
                }
            }

           return result;
        }

        private bool TryGetIndexedEntry(Package package, out IndexedEntry[] entry)
        {
            if (package.TryGetInstallLocation(out var path))
            {
                var file = path.CombineWithFilePath(new FilePath("AppxManifest.xml"));
                if (_fileSystem.File.Exists(file))
                {
                    var manifest = _reader.Read(file);

                    entry = new IndexedEntry[manifest.Applications.Count];
                    for (var index = 0; index < manifest.Applications.Count; index++)
                    {
                        var app = manifest.Applications[index];

                        entry[index] = new UwpIndexSourceEntry(
                            $"{manifest.Id}.{app.Id}",
                            app.AppUserModelId,
                            app.DisplayName,
                            GetIcon(manifest, app),
                            manifest.Description ?? manifest.Publisher);
                    }

                    return true;
                }
            }

            entry = default;
            return false;
        }

        private static Uri GetIcon(AppxManifest manifest, AppxManifestApplication app)
        {
            var icon = manifest.Logo ?? app.Logo
                       ?? app.Images.FirstOrDefault()
                       ?? app.SmallLogo;

            return icon?.Path.ToUri("uwp", new Dictionary<string, string>
            {
                { "bg", app.BackgroundColor }
            });
        }
    }
}
