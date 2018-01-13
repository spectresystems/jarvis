using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.Common.QueryLanguage;
using Jarvis.Core;
using Jarvis.Core.Scoring;

namespace Jarvis.Addin.StackExchange
{
    public abstract class StackExchangeProvider<TQueryResult> : QueryProvider<TQueryResult>
        where TQueryResult : StackExchangeResult, new()
    {
        private readonly IStackExchangeClient _stackExchangeClient;
        private readonly IQueryParser<SearchQuery> _queryParser;
        private readonly IQuestionDescriptionFactory _descriptionFactory;
        protected abstract string Site { get;  }

        protected StackExchangeProvider(IStackExchangeClient stackExchangeClient, IQueryParser<SearchQuery> queryParser, IQuestionDescriptionFactory descriptionFactory)
        {
            _stackExchangeClient = stackExchangeClient;
            _queryParser = queryParser;
            _descriptionFactory = descriptionFactory;
        }

        protected override Task ExecuteAsync(TQueryResult result)
        {
            Process.Start(result.Uri.AbsoluteUri);
            return Task.CompletedTask;
        }

        public override Task<IEnumerable<IQueryResult>> QueryAsync(Query query)
        {
            return QueryAsync(query, CancellationToken.None);
        }

        public async Task<IEnumerable<IQueryResult>> QueryAsync(Query query, CancellationToken ct)
        {
            var searchQuery = _queryParser.Bind(query.Argument, out var freeText);
            searchQuery.Site = Site;
            if (string.IsNullOrEmpty(searchQuery.InTitle))
            {
                searchQuery.InTitle = freeText;
            }
            var questions = await _stackExchangeClient.SearchAsync(searchQuery, ct);
            if (!questions.Any())
            {
                return CreateFallbackResult(searchQuery);
            }
            return questions.Select(question => ConvertQuestion(question, query));
        }

        protected abstract IEnumerable<TQueryResult> CreateFallbackResult(SearchQuery query);

        protected virtual TQueryResult ConvertQuestion(Question question, Query query)
        {
            return new TQueryResult
            {
                Title = question.Title,
                Description = _descriptionFactory.Create(question),
                Uri = question.Link,
                Type = QueryResultType.Other,
                Distance = LevenshteinScorer.Distance(query.Argument, question.Title),
                Score = LevenshteinScorer.Score(query.Argument, question.Title)
            };
        }
    }
}
