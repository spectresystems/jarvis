// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using JetBrains.Annotations;
using IQueryProvider = Jarvis.Core.IQueryProvider;

namespace Jarvis.Services
{
    [UsedImplicitly]
    public sealed class QueryProviderService
    {
        private readonly IJarvisLog _logger;
        private readonly Dictionary<Type, IQueryProvider> _providers;
        private readonly ILookup<string, IQueryProvider> _providersByPrefix;

        public QueryProviderService(IEnumerable<IQueryProvider> providers, IJarvisLog logger)
        {
            _logger = logger;
            var queryProviders = providers as IQueryProvider[] ?? providers.ToArray();
            _providers = queryProviders.ToDictionary(x => x.QueryType, x => x);
            _providersByPrefix = queryProviders.Where(x => x.Command != null).ToLookup(x => x.Command, x => x, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<ImageSource> LoadIcon(IQueryResult result)
        {
            if (_providers.TryGetValue(result.GetType(), out var provider))
            {
                return await provider.GetIconAsync(result);
            }
            return null;
        }

        public async Task Query(Query query, IList<IQueryResult> target)
        {
            using (_logger.BeginScope(LogProperties.QueryId, Guid.NewGuid()))
            using (_logger.TimedOperation("Query {rawQuery}", query.Raw))
            {
                // Query all search providers.
                var providers = GetProviders(query);
                var tasks = providers.Select(async provider =>
                {
                    IQueryResult[] result;
                    using (_logger.TimedOperation(LogLevel.Verbose, "{providerName} query", provider.GetType().Name))
                    {
                        result = (await provider.QueryAsync(query)).ToArray();
                    }

                    // Remove items.
                    for (var i = target.Count - 1; i >= 0; i--)
                    {
                        var current = target[i];
                        if (!result.Contains(current))
                        {
                            target.Remove(target[i]);
                        }
                    }

                    // Add new items.
                    foreach (var item in result)
                    {
                        if (!target.Contains(item))
                        {
                            target.Add(item);
                        }
                        else
                        {
                            // Same item but higher score?
                            if (Math.Abs(target[target.IndexOf(item)].Score - item.Score) > 0.00001f)
                            {
                                target.Remove(item);
                                target.Add(item);
                            }
                        }
                    }
                });

                await Task.WhenAll(tasks);
            }
        }

        public async Task Execute(IQueryResult result)
        {
            if (_providers.TryGetValue(result.GetType(), out var provider))
            {
                await provider.ExecuteAsync(result);
            }
        }

        private IEnumerable<IQueryProvider> GetProviders(Query query)
        {
            if (query.Command != null)
            {
                if (_providersByPrefix.Contains(query.Command))
                {
                    return _providersByPrefix[query.Command];
                }
            }
            return _providers.Values.Where(p => p.Command == null);
        }
    }
}
