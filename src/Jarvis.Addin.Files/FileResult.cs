// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;

namespace Jarvis.Addin.Files
{
    [DebuggerDisplay("{" + nameof(Path) + ",nq}")]
    internal sealed class FileResult : IQueryResult, IEquatable<FileResult>
    {
        public QueryResultType Type { get; }
        public Uri Path { get; }
        public Uri Icon { get; }
        public string Title { get; }
        public string Description { get; }
        public float Distance { get; }
        public float Score { get; }
        public IndexedEntry OriginalEntry { get; }

        public FileResult(QueryResultType type, Uri path, Uri icon,
            string title, string description, float distance, float score, IndexedEntry originalEntry)
        {
            Type = type;
            Path = path;
            Icon = icon;
            Title = title;
            Description = description;
            Distance = distance;
            Score = score;
            OriginalEntry = originalEntry;
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == GetType()
                && Equals((FileResult)obj));
        }

        public bool Equals(IQueryResult obj)
        {
            return obj != null && obj.GetType() == GetType()
                && Equals((FileResult)obj);
        }

        public bool Equals(FileResult other)
        {
            return other?.Path?.Equals(Path) ?? false;
        }

        public override int GetHashCode()
        {
            return Path?.GetHashCode() ?? 0;
        }
    }
}