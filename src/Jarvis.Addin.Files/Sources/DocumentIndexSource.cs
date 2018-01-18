// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Jarvis.Addin.Files.Extensions;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Sources
{
    [UsedImplicitly]
    internal sealed class DocumentIndexSource : IFileIndexSource
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISettingsStore _settings;
        private readonly IJarvisLog _log;

        public string Name => "User document";

        public DocumentIndexSource(IFileSystem fileSystem, ISettingsStore settings, IJarvisLog log)
        {
            _fileSystem = fileSystem;
            _settings = settings;
            _log = log;
        }

        public IEnumerable<IndexedEntry> Index()
        {
            var settings = FileSettings.Load(_settings);
            var extensions = new HashSet<string>(settings.IncludedExtensions, StringComparer.OrdinalIgnoreCase);
            var regexes = settings.ExcludedPatterns.Select(s => new Regex(s, RegexOptions.Singleline)).ToList();

            foreach (var folder in settings.IncludedFolders)
            {
                foreach (var entry in Index(folder, settings.ExcludedFolders, extensions, regexes))
                {
                    yield return entry;
                }
            }
        }

        private IEnumerable<IndexedEntry> Index(
            DirectoryPath path,
            HashSet<DirectoryPath> excludedFolders,
            HashSet<string> includedExtensions,
            List<Regex> excludedPatterns)
        {
            var stack = new Stack<DirectoryPath>();
            stack.Push(path);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                // Folder excluded?
                if (excludedFolders.Contains(current))
                {
                    _log.Debug($"Folder '{current.FullPath}' has been excluded.");
                    continue;
                }

                // Folder name not allowed?
                if (TryMatchPattern(current, excludedPatterns, out var pattern))
                {
                    _log.Debug($"Folder '{current.FullPath}' matched pattern '{pattern}' that has been excluded.");
                    continue;
                }

                var directory = _fileSystem.GetDirectorySafe(current);
                if (directory != null)
                {
                    // Index the directory.
                    yield return DocumentIndexSourceEntry.Directory(directory.Path,
                        directory.Path.GetDirectoryName(), directory.Path.FullPath,
                        directory.Path.ToUri("shell", new Dictionary<string, string>
                        {
                            { "isDirectory", "true" }
                        }));

                    // Get all files in the directory.
                    var files = _fileSystem.GetFilesSafe(directory.Path, "*.*", SearchScope.Current);
                    if (files == null)
                    {
                        continue;
                    }

                    // Index all files in the directory.
                    foreach (var file in files)
                    {
                        if (file.Path.GetFilename().FullPath.StartsWith("."))
                        {
                            // TODO: Temporary fix due to Spectre.System lib.
                            continue;
                        }
                        if ((file.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            continue;
                        }
                        if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            continue;
                        }
                        if (!includedExtensions.Contains(file.Path.GetExtension().Name))
                        {
                            continue;
                        }

                        yield return DocumentIndexSourceEntry.File(file.Path,
                            file.Path.GetFilename().FullPath, file.Path.FullPath,
                            file.Path.ToUri("shell"));
                    }

                    // Push immediate child directories to the stack.
                    directory.GetDirectories("*", SearchScope.Current)
                        .ForEach(child => stack.Push(child.Path));
                }
            }
        }

        private static bool TryMatchPattern(DirectoryPath current, IEnumerable<Regex> excludedPatterns, out Regex pattern)
        {
            pattern = null;
            var currentName = current.GetDirectoryName();
            foreach (var excludedPattern in excludedPatterns)
            {
                if (excludedPattern.IsMatch(currentName))
                {
                    pattern = excludedPattern;
                    return true;
                }
            }
            return false;
        }
    }
}
