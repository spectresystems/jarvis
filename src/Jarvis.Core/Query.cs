// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace Jarvis.Core
{
    public sealed class Query
    {
        public string Command { get; }
        public string Argument { get; }
        public string Raw { get; }

        public Query(string query)
        {
            Raw = query;

            // Get the command part and the argument.
            var parts = Raw?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Command = parts?.FirstOrDefault();
            Argument = string.Join(" ", parts?.Skip(1) ?? Enumerable.Empty<string>()).Trim();
        }

        public override string ToString()
        {
            return Raw;
        }
    }
}
