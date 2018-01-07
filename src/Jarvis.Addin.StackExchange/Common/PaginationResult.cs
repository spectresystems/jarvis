using System.Collections.Generic;

namespace Jarvis.Addin.StackExchange.Common
{
    public class PaginationResult<TItem>
    {
        public List<TItem> Items { get; set; }
        public bool HasMore { get; set; }
        public ulong QuotaMax { get; set; }
        public ulong QuotaRemaining { get; set; }
    }
}
