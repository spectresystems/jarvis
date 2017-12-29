// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;

namespace Jarvis.Addin.Files.Sources
{
    internal sealed class UwpIndexSourceEntry : IndexedEntry
    {
        public string Id { get; }
        public string AppUserModelId { get; }

        public UwpIndexSourceEntry(string id, string appUserModelId, string title, Uri icon, string description)
            : base(QueryResultType.Application, title, description, icon)
        {
            Id = id;
            AppUserModelId = appUserModelId;
        }

        public override FileResult GetFileResult(string query, float distance, float score)
        {
            return new FileResult(
                QueryResultType.Application,
                new Uri($"uwp:///{WebUtility.UrlEncode(Id)}?aumid={WebUtility.UrlEncode(AppUserModelId)}"), Icon,
                Title, Description, distance, score);
        }

        protected override int GetEntryHashCode()
        {
            return Id.GetHashCode();
        }
    }
}