// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jarvis.Core
{
    public abstract class QueryProvider<TResult, TSettings> : QueryProvider<TResult>
        where TResult : IQueryResult
        where TSettings : ISettings
    {
        public override Type SettingsType => typeof(TSettings);
    }

    public abstract class QueryProvider<TResult> : IQueryProvider
        where TResult : IQueryResult
    {
        public virtual string Command => null;
        public virtual Type SettingsType => null;
        Type IQueryProvider.QueryType => typeof(TResult);

        Task<ImageSource> IQueryProvider.GetIconAsync(IQueryResult result)
        {
            return GetIconAsync((TResult)result);
        }

        Task IQueryProvider.ExecuteAsync(IQueryResult result)
        {
            return ExecuteAsync((TResult)result);
        }

        protected abstract Task<ImageSource> GetIconAsync(TResult result);
        protected abstract Task ExecuteAsync(TResult result);

        public abstract Task<IEnumerable<IQueryResult>> QueryAsync(Query query);
    }
}
