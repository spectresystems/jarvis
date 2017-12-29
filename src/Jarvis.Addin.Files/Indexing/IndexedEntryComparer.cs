// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Jarvis.Addin.Files.Indexing
{
    internal sealed class IndexedEntryComparer : IEqualityComparer<IndexedEntry>
    {
        public bool Equals(IndexedEntry x, IndexedEntry y)
        {
            if (x == null && y == null)
            {
                return false;
            }
            return x?.Equals(y) ?? false;
        }

        public int GetHashCode(IndexedEntry obj)
        {
            return obj.GetHashCode();
        }
    }
}
