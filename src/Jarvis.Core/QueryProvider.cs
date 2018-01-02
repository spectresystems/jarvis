// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jarvis.Core
{
    public abstract class QueryProvider<T> : IQueryProvider
        where T : IQueryResult
    {
        public virtual string Command => null;
        Type IQueryProvider.QueryType => typeof(T);

        Task<ImageSource> IQueryProvider.GetIconAsync(IQueryResult result)
        {
            return GetIconAsync((T)result);
        }

        Task IQueryProvider.ExecuteAsync(IQueryResult result)
        {
            return ExecuteAsync((T)result);
        }

        protected abstract Task<ImageSource> GetIconAsync(T result);
        protected abstract Task ExecuteAsync(T result);

        public abstract Task<IEnumerable<IQueryResult>> QueryAsync(Query query);
    }
}
