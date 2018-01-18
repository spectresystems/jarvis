// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Jarvis.Core;
using Spectre.System.IO;

namespace Jarvis.Addin.Files
{
    internal sealed class FileSettings
    {
        public int Version { get; set; }
        public HashSet<DirectoryPath> IncludedFolders { get; }
        public HashSet<string> IncludedExtensions { get; }
        public HashSet<DirectoryPath> ExcludedFolders { get; }
        public HashSet<string> ExcludedPatterns { get; }

        public const int CurrentVersion = 1;

        public FileSettings()
        {
            IncludedFolders = new HashSet<DirectoryPath>(new PathComparer(false));
            IncludedExtensions = new HashSet<string>(StringComparer.Ordinal);
            ExcludedFolders = new HashSet<DirectoryPath>(new PathComparer(false));
            ExcludedPatterns = new HashSet<string>(StringComparer.Ordinal);
        }

        public static FileSettings Load(ISettingsStore settings)
        {
            var model = new FileSettings
            {
                Version = settings.Get<int>(Constants.Settings.Version)
            };

            // Load included folders.
            var includeFolders = settings.Get<string>(Constants.Settings.Include.Folders);
            if (includeFolders != null)
            {
                model.IncludedFolders.AddRange(includeFolders
                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new DirectoryPath(s)));
            }

            // Load included extensions.
            var includeExtensions = settings.Get<string>(Constants.Settings.Include.Extensions);
            if (!string.IsNullOrWhiteSpace(includeExtensions))
            {
                model.IncludedExtensions.AddRange(includeExtensions.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            }

            // Load excluded folders.
            var excludeFolders = settings.Get<string>(Constants.Settings.Exclude.Folders);
            if (excludeFolders != null)
            {
                model.ExcludedFolders.AddRange(excludeFolders
                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new DirectoryPath(s)));
            }

            // Load excluded patterns.
            var excludePatterns = settings.Get<string>(Constants.Settings.Exclude.Patterns);
            if (!string.IsNullOrWhiteSpace(excludePatterns))
            {
                model.ExcludedPatterns.AddRange(excludePatterns.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            }

            return model;
        }

        public void Save(ISettingsStore settings)
        {
            settings.Set(Constants.Settings.Version, CurrentVersion);
            settings.Set(Constants.Settings.Include.Folders, string.Join("|", IncludedFolders.Select(p => p.FullPath)));
            settings.Set(Constants.Settings.Include.Extensions, string.Join("|", IncludedExtensions));
            settings.Set(Constants.Settings.Exclude.Folders, string.Join("|", ExcludedFolders.Select(p => p.FullPath)));
            settings.Set(Constants.Settings.Exclude.Patterns, string.Join("|", ExcludedPatterns));
        }
    }
}
