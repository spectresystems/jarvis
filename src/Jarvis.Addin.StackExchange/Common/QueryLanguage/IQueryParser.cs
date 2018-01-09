using System.Collections.Generic;

namespace Jarvis.Addin.StackExchange.Common.QueryLanguage
{
    public interface IQueryParser<out TOption> where TOption : new()
    {
        /// <summary>
        /// Binds query to options objects.
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="unbound">A dictionary containing keys that can not be bound to the object</param>
        /// <param name="unmatched">A string containing everything that is not identified as key/value</param>
        /// <returns></returns>
        TOption Bind(string query, out IDictionary<string, IList<string>> unbound, out string unmatched);
    }
}