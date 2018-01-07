using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Core;
using Jarvis.Core.Scoring;

namespace Jarvis.Addin.StackExchange
{
    public abstract class StackExchangeProvider<TQueryResult> : QueryProvider<TQueryResult>
        where TQueryResult : StackExchangeResult, new()
    {
        private readonly IStackExchangeClient _stackExchangeClient;
        protected abstract string Site { get;  }

        protected StackExchangeProvider(IStackExchangeClient stackExchangeClient)
        {
            _stackExchangeClient = stackExchangeClient;
        }

        protected override Task ExecuteAsync(TQueryResult result)
        {
            Process.Start(result.Uri.AbsoluteUri);
            return Task.CompletedTask;
        }

        public override async Task<IEnumerable<IQueryResult>> QueryAsync(Query query)
        {
            var questions = await _stackExchangeClient.SearchAsync(Site, query.Argument, CancellationToken.None);
            return questions.Select(question => ConvertQuestion(question, query));
        }

        protected virtual TQueryResult ConvertQuestion(Question question, Query query)
        {
            return new TQueryResult
            {
                Description = question.Title,
                Uri = question.Link,
                Title = question.Title,
                Type = QueryResultType.Other,
                Distance = LevenshteinScorer.Distance(query.Argument, question.Title),
                Score = LevenshteinScorer.Score(query.Argument, question.Title)
            };
        }
    }
}
