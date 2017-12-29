// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace Jarvis.Core.Scoring
{
    public static class LevenshteinScorer
    {
        public static float Distance(string source, string target)
        {
            return Score(source, target, false);
        }

        public static float Score(string source, string target)
        {
            return Score(source, target, true);
        }

        public static float Score(string source, string target, bool normalize)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));

            source = source.ToLowerInvariant();
            target = target.ToLowerInvariant();

            var n = source.Length;
            var m = target.Length;

            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }

            int[,] d = new int[n + 1, m + 1];
            Enumerable.Range(0, n + 1).ForEach(i => d[i, 0] = i);
            Enumerable.Range(0, m + 1).ForEach(i => d[0, i] = i);

            for (var sourceIndex = 1; sourceIndex <= n; sourceIndex++)
            {
                for (var targetIndex = 1; targetIndex <= m; targetIndex++)
                {
                    var cost = (target[targetIndex - 1] == source[sourceIndex - 1]) ? 0 : 1;
                    d[sourceIndex, targetIndex] = Math.Min(
                        Math.Min(d[sourceIndex - 1, targetIndex] + 1, d[sourceIndex, targetIndex - 1] + 1),
                        d[sourceIndex - 1, targetIndex - 1] + cost);
                }
            }

            return normalize
                ? d[n, m] / (float)n
                : d[n, m];
        }
    }
}
