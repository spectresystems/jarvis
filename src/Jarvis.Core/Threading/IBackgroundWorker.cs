// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Core.Threading
{
    public interface IBackgroundWorker
    {
        string Name { get; }
        bool Enabled();
        Task<bool> Run(CancellationToken token);
    }
}