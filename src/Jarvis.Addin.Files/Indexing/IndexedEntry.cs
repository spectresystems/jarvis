// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Jarvis.Core;

namespace Jarvis.Addin.Files.Indexing
{
    [DebuggerDisplay("{Title,nq}")]
    internal abstract class IndexedEntry : IEquatable<IndexedEntry>
    {
        public QueryResultType Type { get; }
        public string Description { get; }
        public string Title { get; }
        public Uri Icon { get; }

        protected IndexedEntry(QueryResultType type, string title, string description, Uri icon)
        {
            Type = type;
            Title = title;
            Icon = icon;
            Description = description;
        }

        public bool Equals(IndexedEntry other)
        {
            return GetHashCode() == (other?.GetHashCode() ?? 0);
        }

        protected abstract int GetEntryHashCode();

        public abstract FileResult GetFileResult(string query, float distance, float score);

        public override bool Equals(object obj)
        {
            return obj is IndexedEntry file && Equals(file);
        }

        // https://stackoverflow.com/a/263416/936
        public sealed override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)2166136261;
                hash = (hash * 16777619) ^ GetType().GetHashCode();
                hash = (hash * 16777619) ^ GetEntryHashCode();
                return hash;
            }
        }
    }
}