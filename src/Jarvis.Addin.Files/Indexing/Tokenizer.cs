// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jarvis.Addin.Files.Indexing
{
    internal static class Tokenizer
    {
        public static IEnumerable<string> Tokenize(string word)
        {
            var builder = new StringBuilder();
            var reader = new StringReader(word.Trim());
            while (reader.Peek() != -1)
            {
                var current = (char)reader.Peek();
                if (char.IsWhiteSpace(current))
                {
                    if (builder.Length > 0)
                    {
                        yield return builder.ToString();
                    }
                    builder.Clear();
                    reader.Read();
                    continue;
                }
                builder.Append(current);
                reader.Read();
            }

            if (builder.Length > 0)
            {
                yield return builder.ToString();
            }
        }
    }
}
