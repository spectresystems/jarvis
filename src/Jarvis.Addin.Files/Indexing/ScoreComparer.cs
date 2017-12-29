// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Jarvis.Core;

namespace Jarvis.Addin.Files.Indexing
{
    internal class ScoreComparer : Comparer<IQueryResult>
    {
        public override int Compare(IQueryResult x, IQueryResult y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return x.Score.CompareTo(y.Score);
        }
    }
}