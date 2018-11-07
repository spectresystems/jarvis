// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using IWshRuntimeLibrary;
using Jarvis.Addin.Files.Extensions;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Spectre.System.IO;
using IFile = Spectre.System.IO.IFile;
using IFileSystem = Spectre.System.IO.IFileSystem;

namespace Jarvis.Addin.Files.Sources
{
    [UsedImplicitly]
    internal sealed class StartMenuIndexSource : IFileIndexSource
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJarvisLog _log;

        public string Name => "Start menu";

        public StartMenuIndexSource(IFileSystem fileSystem, IJarvisLog log)
        {
            _fileSystem = fileSystem;
            _log = new LogDecorator(nameof(StartMenuIndexSource), log);
        }

        public IEnumerable<IndexedEntry> Index()
        {
            var common = GetFilesInSpecialFolder(Environment.SpecialFolder.CommonStartMenu);
            var roaming = GetFilesInSpecialFolder(Environment.SpecialFolder.StartMenu);

            var files = common.Concat(roaming);
            foreach (var file in files)
            {
                var shortcut = GetShortcut(file);
                var targetPath = !string.IsNullOrWhiteSpace(shortcut?.TargetPath)
                    ? new FilePath(shortcut.TargetPath) : null;

                yield return new StartMenuIndexSourceEntry(
                    file.Path,
                    targetPath,
                    file.Path.GetFilenameWithoutExtension().FullPath,
                    file.Path.ToUri("shell"),
                    SafeCom(() => string.IsNullOrWhiteSpace(shortcut?.Description)
                        ? targetPath?.FullPath?.Replace('\\', '/') ?? file.Path.FullPath
                        : shortcut.Description?.Replace('\\', '/')));
            }
        }

        private IEnumerable<IFile> GetFilesInSpecialFolder(Environment.SpecialFolder folder)
        {
            try
            {
                var directory = _fileSystem.GetDirectory(new DirectoryPath(Environment.GetFolderPath(folder)));
                return directory.GetFiles("*.lnk", SearchScope.Recursive)
                    .Concat(directory.GetFiles("*.url", SearchScope.Recursive));
            }
            catch (Exception ex)
            {
                _log.Error($"An error occured while indexing '{folder}': {ex.Message}");
                return Enumerable.Empty<IFile>();
            }
        }

        private static IWshShortcut GetShortcut(IFile file)
        {
            try
            {
                var extension = file.Path.GetExtension().Name;
                if (extension != "lnk" && extension != "url")
                {
                    return null;
                }

                var shell = new WshShell();
                return shell.CreateShortcut(file.Path.FullPath) as IWshShortcut;
            }
            catch (COMException)
            {
                return null;
            }
        }

        private static T SafeCom<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (COMException)
            {
                return default;
            }
        }
    }
}
