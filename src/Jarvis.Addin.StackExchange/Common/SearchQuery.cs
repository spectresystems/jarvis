using System;
using System.Collections.Generic;
using System.Linq;

namespace Jarvis.Addin.StackExchange.Common
{
    public class SearchQuery
    {
        public string Site { get; set; }
        public uint Page { get; set; }
        public uint PageSize { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public SortType Sort { get; set; }
        public SortOrder Order { get; set; }
        public string InTitle { get; set; }
        public IEnumerable<string> Tagged { get; set; }
        public IEnumerable<string> NotTagged { get; set; }

        public SearchQuery()
        {
            Tagged = Enumerable.Empty<string>();
            NotTagged = Enumerable.Empty<string>();
        }

        public enum SortType
        {
            Activity,
            Creation,
            Relevance,
            Votes
        }

        public enum SortOrder
        {
            Descending,
            Ascending
        }
    }
}
