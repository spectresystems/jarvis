// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jarvis.Core
{
    public interface IQueryProvider
    {
        string Command { get; }
        Type QueryType { get; }
        Type SettingsType { get; }

        Task<ImageSource> GetIconAsync(IQueryResult result);
        Task<IEnumerable<IQueryResult>> QueryAsync(Query query);
        Task ExecuteAsync(IQueryResult result);
    }
}
