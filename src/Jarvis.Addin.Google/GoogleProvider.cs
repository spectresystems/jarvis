// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Jarvis.Addin.Google
{
    [UsedImplicitly]
    internal sealed class GoogleProvider : QueryProvider<GoogleResult>
    {
        private readonly IJarvisLog _log;
        private readonly HttpClient _client;
        private readonly ImageSource _googleIcon;
        private readonly ImageSource _linkIcon;

        public override string Command => "g";

        public GoogleProvider(IJarvisLog log)
        {
            _log = new LogDecorator(nameof(GoogleProvider), log);
            _client = new HttpClient();
            _googleIcon = new BitmapImage(new Uri("pack://application:,,,/Jarvis.Addin.Google;component/Resources/Google.png"));
            _linkIcon = new BitmapImage(new Uri("pack://application:,,,/Jarvis.Addin.Google;component/Resources/Link.png"));
        }

        protected override Task<ImageSource> GetIconAsync(GoogleResult result)
        {
            return Task.FromResult(result.IsGoogleQuery ? _googleIcon : _linkIcon);
        }

        public override async Task<IEnumerable<IQueryResult>> QueryAsync(Query query)
        {
            Ensure.NotNull(query, nameof(query));

            if (string.IsNullOrWhiteSpace(query.Argument))
            {
                var uri = new Uri("https://google.com");
                return new[]
                {
                    (IQueryResult)new GoogleResult(
                        uri, true, "Open Google in browser",
                        uri.AbsoluteUri, 0, 0)
                };
            }

            try
            {
                var uri = $"https://www.google.com/complete/search?output=chrome&q={WebUtility.UrlEncode(query.Argument)}";
                var result = await _client.GetAsync(uri);

                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    var queryResults = JArray.Parse(data)[1]
                        .Where(x => x.Type == JTokenType.String).Select(x => x.Value<string>())
                        .ToList();

                    return queryResults.Any()
                        ? queryResults.Select(x => GoogleResult.Create(query, x))
                        : new[] { GoogleResult.Create(query, description: $"Search Google for '{query.Argument}'") };
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "An error occured while querying Google.");
            }

            return Enumerable.Empty<IQueryResult>();
        }

        protected override Task ExecuteAsync(GoogleResult result)
        {
            Process.Start(result.Uri.AbsoluteUri);
            return Task.CompletedTask;
        }
    }
}
