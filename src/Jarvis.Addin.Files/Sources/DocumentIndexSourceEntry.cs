// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Jarvis.Addin.Files.Extensions;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Sources
{
    internal sealed class DocumentIndexSourceEntry : IndexedEntry, IHasPath
    {
        public Path Path { get; }
        public bool IsDirectory { get; set; }

        public DocumentIndexSourceEntry(
            QueryResultType type, Path path, string title, Uri icon, string description)
            : base(type, title, description, icon)
        {
            Path = path;
            IsDirectory = type == QueryResultType.Folder;
        }

        public static DocumentIndexSourceEntry File(
            FilePath path, string title, string description, Uri icon)
        {
            return new DocumentIndexSourceEntry(
                QueryResultType.File, path, title, icon, description);
        }

        public static DocumentIndexSourceEntry Directory(
            DirectoryPath path, string title, string description, Uri icon)
        {
            return new DocumentIndexSourceEntry(
                QueryResultType.Folder, path, title, icon, description);
        }

        public override FileResult GetFileResult(string query, float distance, float score)
        {
            return new FileResult(
                IsDirectory ? QueryResultType.Folder : QueryResultType.File,
                Path.ToUri("shell"),
                Icon, Title, Description, distance, score);
        }

        protected override int GetEntryHashCode()
        {
            return Path.GetHashCode();
        }
    }
}