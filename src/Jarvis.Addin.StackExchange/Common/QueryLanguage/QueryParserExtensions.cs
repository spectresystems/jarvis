namespace Jarvis.Addin.StackExchange.Common.QueryLanguage
{
    public static class QueryParserExtensions
    {
        public static TOption Bind<TOption>(this IQueryParser<TOption> parser, string query) where TOption : new()
        {
            return parser.Bind(query, out _, out _);
        }

        public static TOption Bind<TOption>(this IQueryParser<TOption> parser, string query, out string unmatched) where TOption : new()
        {
            return parser.Bind(query, out _, out unmatched);
        }
    }
}