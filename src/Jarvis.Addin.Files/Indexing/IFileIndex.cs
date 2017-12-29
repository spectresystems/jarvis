// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Core;

namespace Jarvis.Addin.Files.Indexing
{
    internal interface IFileIndex
    {
        Task<IEnumerable<IQueryResult>> Find(string query, CancellationToken token);
    }
}
