// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Spectre.System.IO;

namespace Jarvis.Infrastructure.Utilities
{
    public static class PathUtility
    {
        public static DirectoryPath DataPath { get; set; }
        public static DirectoryPath LogPath { get; set; }
        public static DirectoryPath InstallerPath { get; set; }

        public static void Initialize()
        {
            DataPath = new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                .Combine(new DirectoryPath("Jarvis"));

            LogPath = DataPath.Combine(new DirectoryPath("Logs"));
            InstallerPath = DataPath.Combine(new DirectoryPath("Installers"));

            var fileSystem = new FileSystem();
            CreatePath(fileSystem, DataPath);
            CreatePath(fileSystem, LogPath);
            CreatePath(fileSystem, InstallerPath);
        }

        private static void CreatePath(IFileSystem fileSystem, DirectoryPath path)
        {
            if (!fileSystem.Directory.Exists(path))
            {
                fileSystem.Directory.Create(path);
            }
        }
    }
}
