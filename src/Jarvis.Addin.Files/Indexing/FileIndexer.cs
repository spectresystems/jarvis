﻿// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Addin.Files.Collections;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Scoring;
using Jarvis.Core.Threading;
using JetBrains.Annotations;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Indexing
{
    [UsedImplicitly]
    internal sealed class FileIndexer : IBackgroundWorker, IFileIndex
    {
        private readonly IJarvisLog _log;
        private readonly List<IFileIndexSource> _sources;
        private readonly HashSet<string> _stopWords;
        private readonly ScoreComparer _comparer;
        private readonly IndexedEntryComparer _entryComparer;

        private Trie<IndexedEntry> _trie;

        public string Name => "File indexing service";

        public FileIndexer(IEnumerable<IFileIndexSource> sources, IJarvisLog log)
        {
            _log = log;
            _sources = new List<IFileIndexSource>(sources ?? Array.Empty<IFileIndexSource>());
            _stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "to", "the" };
            _comparer = new ScoreComparer();
            _entryComparer = new IndexedEntryComparer();
        }

        public Task<bool> Run(CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    var st = new Stopwatch();
                    st.Start();

                    var result = LoadResults(token);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    _log.Debug("Updating index...");
                    var st2 = new Stopwatch();
                    st2.Start();

                    var trie = new Trie<IndexedEntry>();
                    foreach (var file in result)
                    {
                        // Index individual words as well as combinations.
                        Index(trie, file.Title, file);
                        Index(trie, file.Description, file);

                        // Also index the whole words without tokenization.
                        trie.Insert(file.Title, file);

                        // Is this a file path?
                        if (file is IHasPath entryWithPath)
                        {
                            if (entryWithPath.Path is FilePath filePath)
                            {
                                // Index individual words as well as combinations.
                                Index(trie, filePath.GetFilenameWithoutExtension().FullPath, file);
                                Index(trie, filePath.GetFilename().RemoveExtension().FullPath, file);

                                // Also index the whole words without tokenization.
                                trie.Insert(filePath.GetFilenameWithoutExtension().FullPath, file);
                                trie.Insert(filePath.GetFilename().FullPath, file);
                            }
                        }
                    }

                    st2.Stop();
                    _log.Debug($"Building trie took {st2.ElapsedMilliseconds}ms");

                    _log.Debug("Writing index...");
                    Interlocked.Exchange(ref _trie, trie);

                    _log.Verbose($"Nodes: {_trie.NodeCount}");
                    _log.Verbose($"Items: {_trie.ItemCount}");

                    // Wait for a minute.
                    st.Stop();
                    _log.Debug($"Indexing done. Took {st.ElapsedMilliseconds}ms");

                    if (token.WaitHandle.WaitOne((int)TimeSpan.FromMinutes(5).TotalMilliseconds))
                    {
                        _log.Information("We were instructed to stop (2).");
                        break;
                    }
                }

                return true;
            });
        }

        private List<IndexedEntry> LoadResults(CancellationToken token)
        {
            // Ask all sources for files.
            return _sources.AsParallel()
                .SelectMany(src =>
                {
                    if (token.IsCancellationRequested)
                    {
                        _log.Information("We were instructed to stop (1). Aborting indexing...");
                        return Enumerable.Empty<IndexedEntry>();
                    }

                    _log.Debug($"Running '{src.Name}' indexer...");
                    return src.Index();
                })
                .ToList();
        }

        private void Index(Trie<IndexedEntry> trie, string word, IndexedEntry entry)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return;
            }

            var parts = Tokenizer.Tokenize(word).ToArray();
            for (var i = 0; i < parts.Length; i++)
            {
                if (!_stopWords.Contains(parts[i]) && parts[i].Length > 2)
                {
                    trie.Insert(parts[i], entry);
                }
                trie.Insert(string.Join(" ", parts.Skip(i).Take(parts.Length - i)), entry);
            }
        }

        public async Task<IEnumerable<IQueryResult>> Find(string query, CancellationToken token)
        {
            var trie = _trie;
            if (trie == null)
            {
                return Enumerable.Empty<IQueryResult>();
            }

            // Get all matches.
            var result = await trie.Find(query);
            result = result.Concat(await trie.Find(query, 1));

            return new HashSet<IQueryResult>(
                result.SelectMany(x => x.Data)
                    .Distinct(_entryComparer)
                    .Select(entry => entry.GetFileResult(query,
                        CalculateDistance(entry, query),
                        CalculateScore(entry, query))))
                .OrderBy(fileResult => fileResult.Type)
                .ThenBy(item => item, _comparer);
        }

        private static float CalculateDistance(IndexedEntry entry, string query)
        {
#if DEBUG
            return Math.Min(LevenshteinScorer.Distance(entry.Title, query),
                LevenshteinScorer.Distance(entry.Description ?? entry.Title, query));
#else
            return 0;
#endif
        }

        private static float CalculateScore(IndexedEntry entry, string query)
        {
            return Math.Min(LevenshteinScorer.Score(entry.Title, query),
                LevenshteinScorer.Score(entry.Description ?? entry.Title, query));
        }
    }
}