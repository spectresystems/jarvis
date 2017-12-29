// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Jarvis.Addin.Files.Drawing;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;
using JetBrains.Annotations;
using static Jarvis.Addin.Files.Sources.Uwp.ShellInterop;

namespace Jarvis.Addin.Files
{
    [UsedImplicitly]
    internal sealed class FileProvider : QueryProvider<FileResult>
    {
        private readonly IFileIndex _index;
        private readonly IconLoader _loader;

        [UsedImplicitly]
        public Type QueryType => typeof(FileResult);

        public FileProvider(IFileIndex index, IconLoader loader)
        {
            _index = index;
            _loader = loader;
        }

        protected override async Task<ImageSource> GetIcon(FileResult result)
        {
            return await _loader.LoadAsync(result);
        }

        public override async Task<IEnumerable<IQueryResult>> Query(Query query, bool fallback)
        {
            if (query.Raw?.Length <= 1 || fallback)
            {
                return Enumerable.Empty<IQueryResult>();
            }
            return await _index.Find(query.Raw?.Trim(), CancellationToken.None).ConfigureAwait(false);
        }

        protected override Task Execute(FileResult result)
        {
            if (result.Path != null)
            {
                var path = WebUtility.UrlDecode(result.Path.AbsolutePath).TrimStart('/');

                if (result.Path.Scheme == "shell")
                {
                    Process.Start(path);
                }
                else if (result.Path.Scheme == "uwp")
                {
                    var queryString = result.Path.GetQueryString();
                    if (queryString.TryGetValue("aumid", out var values))
                    {
                        var aumid = values?.FirstOrDefault();
                        if (aumid != null)
                        {
                            var manager = new ApplicationActivationManager();
                            manager.ActivateApplication(aumid, null, ACTIVATEOPTIONS.AO_NONE, out var _);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
