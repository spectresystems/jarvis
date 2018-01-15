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
    internal sealed class StartMenuIndexSourceEntry : IndexedEntry, IHasPath
    {
        public FilePath Path { get; }
        public FilePath TargetPath { get; }

        Path IHasPath.Path => Path;

        public StartMenuIndexSourceEntry(FilePath path, FilePath targetPath, string title, Uri icon, string description)
            : base(QueryResultType.Application, title, description, icon)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            TargetPath = targetPath ?? path;
        }

        public override FileResult GetFileResult(string query, float distance, float score)
        {
            return new FileResult(
                QueryResultType.Application,
                Path.ToUri("shell"),
                Icon,
                Title, Description, distance, score,
                this);
        }

        protected override int GetEntryHashCode()
        {
            return TargetPath.FullPath.GetHashCode();
        }
    }
}