// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Net;
using Jarvis.Core;
using Jarvis.Core.Scoring;
using JetBrains.Annotations;

namespace Jarvis.Addin.Google
{
    [UsedImplicitly]
    [DebuggerDisplay("{" + nameof(Title) + ",nq}")]
    internal struct GoogleResult : IQueryResult, IEquatable<GoogleResult>
    {
        public Uri Uri { get; }
        public bool IsGoogleQuery { get; }
        public string Title { get; }
        public string Description { get; }
        public float Distance { get; }
        public float Score { get; }
        public QueryResultType Type => QueryResultType.Other;

        public GoogleResult(Uri uri, bool isGoogleQuery, string title, string description,
            float distance, float score)
        {
            Uri = uri;
            IsGoogleQuery = isGoogleQuery;
            Title = title;
            Description = description;
            Distance = distance;
            Score = score;
        }

        public bool Equals(IQueryResult obj)
        {
            return obj != null && obj.GetType() == GetType()
                && Equals((GoogleResult)obj);
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == GetType()
                && Equals((GoogleResult)obj));
        }

        public bool Equals(GoogleResult other)
        {
            return other.Uri?.Equals(Uri) ?? false;
        }

        public static IQueryResult Create(Query query, string term = null, string description = null)
        {
            Ensure.NotNull(query, nameof(query));

            term = term ?? query.Argument;
            description = description ?? term;

            var isGoogleQuery = !(term.StartsWith("http://") || term.StartsWith("https://"));
            var uri = isGoogleQuery ? new Uri($"https://www.google.se/search?q={WebUtility.UrlEncode(term)}") : new Uri(term);
            return new GoogleResult(uri, isGoogleQuery, description, uri.AbsoluteUri,
                isGoogleQuery ? LevenshteinScorer.Score(term, query.Argument, false) : 0,
                isGoogleQuery ? LevenshteinScorer.Score(term, query.Argument) : 0);
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }
    }
}