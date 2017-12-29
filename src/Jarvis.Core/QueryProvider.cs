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

        Task<ImageSource> IQueryProvider.GetIcon(IQueryResult result)
        {
            return GetIcon((T)result);
        }

        Task IQueryProvider.Execute(IQueryResult result)
        {
            return Execute((T)result);
        }

        protected abstract Task<ImageSource> GetIcon(T result);
        protected abstract Task Execute(T result);

        public abstract Task<IEnumerable<IQueryResult>> Query(Query query, bool fallback);
    }
}
