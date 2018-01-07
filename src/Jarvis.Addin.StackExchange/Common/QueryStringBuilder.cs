using System.Linq;
using System.Net;
using System.Text;

namespace Jarvis.Addin.StackExchange.Common
{
    public interface IQueryStringBuilder
    {
        string Build(SearchQuery query);
    }

    public class QueryStringBuilder : IQueryStringBuilder
    {
        public string Build(SearchQuery query)
        {
            var queryParameters = new StringBuilder();

            if (!string.IsNullOrEmpty(query.InTitle))
            {
                queryParameters.Append($"?intitle={Encode(query.InTitle)}");
            }

            if (query.Tagged.Any())
            {
                var firstChar = queryParameters.Length == 0 ? "?" : "&";
                queryParameters.Append($"{firstChar}{string.Join(";", query.Tagged.Select(Encode))}");
            }

            if (query.NotTagged.Any())
            {
                queryParameters.Append($"&{string.Join(";", query.NotTagged.Select(Encode))}");
            }

            queryParameters.Append($"&site={query.Site}");

            queryParameters.Append("&sort=");
            switch (query.Sort)
            {
                case SearchQuery.SortType.Creation:
                    queryParameters.Append("creation");
                    break;
                case SearchQuery.SortType.Relevance:
                    queryParameters.Append("relevance");
                    break;
                case SearchQuery.SortType.Votes:
                    queryParameters.Append("votes");
                    break;
                default:
                    queryParameters.Append("activity");
                    break;
            }

            queryParameters.Append("&order=");
            switch (query.Order)
            {
                case SearchQuery.SortOrder.Ascending:
                    queryParameters.Append("asc");
                    break;
                default:
                    queryParameters.Append("desc");
                    break;
            }

            if (query.Page != default)
            {
                queryParameters.Append($"&page={query.Page}");
            }

            if (query.PageSize != default)
            {
                queryParameters.Append($"&pagesize={query.PageSize}");
            }

            if (query.FromDate != default)
            {
                queryParameters.Append($"&fromdate={query.FromDate.ToEpoch()}");
            }

            if (query.ToDate != default)
            {
                queryParameters.Append($"&todate={query.ToDate.ToEpoch()}");
            }

            return queryParameters.ToString();
        }

        private static string Encode(string term)
        {
            return WebUtility.UrlEncode(term);
        }
    }
}
