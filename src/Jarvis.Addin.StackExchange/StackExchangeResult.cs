using System;
using Jarvis.Core;

namespace Jarvis.Addin.StackExchange
{
    public abstract class StackExchangeResult : IQueryResult, IEquatable<StackExchangeResult>
    {
        public string Title { get; internal set; }
        public string Description { get; internal set; }
        public Uri Uri { get; internal set; }
        public float Distance { get; internal set; }
        public float Score { get; internal set; }
        public QueryResultType Type { get; internal set; }

        public bool Equals(IQueryResult other)
        {
            return other != null && other.GetType() == GetType()
                   && Equals((StackExchangeResult)other);
        }

        public bool Equals(StackExchangeResult other)
        {
            return other?.Uri?.Equals(Uri) ?? false;
        }

        public override int GetHashCode()
        {
            return Uri?.GetHashCode() ?? -1;
        }
    }
}
