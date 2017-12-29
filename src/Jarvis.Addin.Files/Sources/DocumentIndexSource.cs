// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Jarvis.Addin.Files.Extensions;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;
using JetBrains.Annotations;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Sources
{
    [UsedImplicitly]
    internal sealed class DocumentIndexSource : IFileIndexSource
    {
        private readonly IFileSystem _fileSystem;

        public string Name => "User document";

        public DocumentIndexSource(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<IndexedEntry> Index()
        {
            var folders = new[]
            {
                new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)),
                new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)),
                new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
            };

            foreach (var folder in folders)
            {
                foreach (var entry in Index(folder))
                {
                    yield return entry;
                }
            }
        }

        private IEnumerable<IndexedEntry> Index(DirectoryPath path)
        {
            var stack = new Stack<DirectoryPath>();
            stack.Push(path);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

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
    }
}
