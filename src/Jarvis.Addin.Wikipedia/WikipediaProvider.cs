// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Scoring;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Jarvis.Addin.Wikipedia
{
    [UsedImplicitly]
    internal sealed class WikipediaProvider : QueryProvider<WikipediaResult>
    {
        private readonly IJarvisLog _log;
        private readonly HttpClient _client;
        private readonly ImageSource _icon;

        public override string Command => "wiki";

        public WikipediaProvider(IJarvisLog log)
        {
            _log = log;
            _client = new HttpClient();
            _icon = new BitmapImage(new Uri("pack://application:,,,/Jarvis.Addin.Wikipedia;component/Resources/Wikipedia.png"));
        }

        protected override Task<ImageSource> GetIconAsync(WikipediaResult result)
        {
            return Task.FromResult(_icon);
        }

        public override async Task<IEnumerable<IQueryResult>> QueryAsync(Query query)
        {
            Ensure.NotNull(query, nameof(query));

            if (string.IsNullOrWhiteSpace(query.Argument))
            {
                var uri = new Uri("https://en.wikipedia.org");
                return new[]
                {
                    (IQueryResult)new WikipediaResult(
                    uri, "Open Wikipedia in browser",
                    uri.AbsoluteUri, 0, 0)
                };
            }

            try
            {
                var uri = $"https://en.wikipedia.org/w/api.php?action=opensearch&profile=fuzzy&limit=5&search={Encode(query.Argument)}";
                var result = await _client.GetAsync(uri);

                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();

                    var keywords = JArray.Parse(data)[1].Where(x => x.Type == JTokenType.String).Select(x => x.Value<string>());
                    var uris = JArray.Parse(data)[3].Where(x => x.Type == JTokenType.String).Select(x => new Uri(x.Value<string>()));

                    var results = keywords
                        .Zip(uris, (title, url) =>
                            (IQueryResult)new WikipediaResult(url, title, url.AbsoluteUri,
                                LevenshteinScorer.Score(title, query.Argument, false),
                                LevenshteinScorer.Score(title, query.Argument)))
                        .ToList();

                    if (!results.Any())
                    {
                        return new[] { await CreateFallbackResult(query.Argument) };
                    }

                    return results;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "An error occured while querying Wikipedia.");
            }

            return Enumerable.Empty<IQueryResult>();
        }

        protected override Task<IQueryResult> CreateFallbackResult(string query)
        {
            var fallbackUrl = new Uri($"https://en.wikipedia.org/wiki/{Encode(query)}");
            return Task.FromResult((IQueryResult)new WikipediaResult(
                    fallbackUrl, $"Search Wikipedia for '{query}'",
                    fallbackUrl.AbsoluteUri, 0, 0));
        }

        protected override Task ExecuteAsync(WikipediaResult result)
        {
            Process.Start(result.Uri.AbsoluteUri);
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Encode(string term)
        {
            return WebUtility.UrlEncode(
                term.Replace(" ", "_")
                .Replace("#", "♯"));
        }
    }
}
