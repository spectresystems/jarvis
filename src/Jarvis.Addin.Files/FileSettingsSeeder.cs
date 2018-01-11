// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Jarvis.Core;
using Spectre.System.IO;

namespace Jarvis.Addin.Files
{
    internal sealed class FileSettingsSeeder : ISettingsSeeder
    {
        public void Seed(ISettingsStore store)
        {
            var model = FileSettings.Load(store);

            if (model.Version == 0)
            {
                Initialize(model);
                model.Save(store);
            }
        }

        private static void Initialize(FileSettings model)
        {
            // Included folders.
            model.IncludedFolders.Add(new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            model.IncludedFolders.Add(new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
            model.IncludedFolders.Add(new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));
            model.IncludedFolders.Add(new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

            // Included extensions.
            model.IncludedExtensions.AddRange(new[]
            {
                "ai", "avi", "doc", "docx", "eps", "flv", "gif", "htm", "html",
                "jpeg", "jpg", "mov", "mp3", "mp4", "mpg", "mpeg", "odt", "ogg", "ogv", "pdf", "png", "ppt", "psd",
                "rar", "rtf", "svg", "txt", "wav", "wma", "xls", "xlsx", "zip"
            });

            // Excluded patterns.
            model.ExcludedPatterns.AddRange(new[] { ".git", "node_modules", "packages" });
        }
    }
}
