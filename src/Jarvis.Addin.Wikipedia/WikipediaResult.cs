// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Jarvis.Core;
using JetBrains.Annotations;

namespace Jarvis.Addin.Wikipedia
{
    [UsedImplicitly]
    [DebuggerDisplay("{" + nameof(Title) + ",nq}")]
    internal struct WikipediaResult : IQueryResult, IEquatable<WikipediaResult>
    {
        public Uri Uri { get; }
        public string Title { get; }
        public string Description { get; }
        public float Distance { get; }
        public float Score { get; }
        public QueryResultType Type => QueryResultType.Other;

        public WikipediaResult(Uri uri, string title, string description,
            float distance, float score)
        {
            Uri = uri;
            Title = title;
            Description = description;
            Distance = distance;
            Score = score;
        }

        public bool Equals(IQueryResult obj)
        {
            return obj != null && obj.GetType() == GetType()
                && Equals((WikipediaResult)obj);
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == GetType()
                && Equals((WikipediaResult)obj));
        }

        public bool Equals(WikipediaResult other)
        {
            return other.Uri?.Equals(Uri) ?? false;
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }
    }
}