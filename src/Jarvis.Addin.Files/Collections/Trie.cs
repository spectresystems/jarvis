// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.Addin.Files.Collections
{
    internal sealed class Trie<T>
    {
        private readonly TrieNode<T> _root;

        public int NodeCount { get; private set; }
        public int ItemCount { get; private set; }

        public Trie()
        {
            _root = new TrieNode<T>(null, '\0');
        }

        public void Insert(string word, T data)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentNullException(nameof(word));
            }

            var characters = word.ToLower().ToCharArray();
            var current = _root;
            for (var index = 0; index < characters.Length; index++)
            {
                var child = current.FindNode(characters[index]);
                if (child != null)
                {
                    current = child;
                }
                else
                {
                    current.Children.Add(new TrieNode<T>(current, characters[index]));
                    current = current.FindNode(characters[index]);

                    NodeCount++;
                }

                if (index == characters.Length - 1)
                {
                    current.Word = word;
                    if (!current.Data.Contains(data))
                    {
                        ItemCount++;
                        current.Data.Add(data);
                    }
                }
            }
        }

        public Task<IEnumerable<TrieNode<T>>> Find(string word, bool nonExact = true)
        {
            return Task.Run(() =>
            {
                var result = new List<TrieNode<T>>();

                if (string.IsNullOrEmpty(word))
                {
                    return result;
                }

                var current = _root;

                var letters = word.ToLowerInvariant().ToCharArray();
                for (var index = 0; index < letters.Length; index++)
                {
                    if (current.FindNode(letters[index]) == null)
                    {
                        return result;
                    }
                    current = current.FindNode(letters[index]);
                }

                if (current.Word != null)
                {
                    result.Add(current);
                }

                if (nonExact)
                {
                    // Try finding the next non-exact words.
                    var stack = new Stack<TrieNode<T>>(new[] { current });
                    while (stack.Count > 0)
                    {
                        current = stack.Pop();
                        if (current.Word != null)
                        {
                            result.Add(current);
                        }

                        foreach (var child in current.Children)
                        {
                            stack.Push(child);
                        }
                    }
                }

                return (IEnumerable<TrieNode<T>>)result;
            });
        }

        public Task<IEnumerable<TrieNode<T>>> Find(string word, int maxDistance)
        {
            return Task.Run(() =>
            {
                var result = new List<TrieNode<T>>();
                var previousRow = new List<int>();
                previousRow.AddRange(Enumerable.Range(0, word.Length + 1));
                foreach (var child in _root.Children)
                {
                    Find(child, char.ToLowerInvariant(child.Letter), word.ToLowerInvariant(), previousRow, result, maxDistance);
                }
                return (IEnumerable<TrieNode<T>>)result;
            });
        }

        private static void Find(
            TrieNode<T> node, char letter, string word,
            IReadOnlyList<int> previousRow, ICollection<TrieNode<T>> result,
            int maxDistance)
        {
            var columns = word.Length + 1;
            var currentRow = new List<int> { previousRow[0] + 1 };

            // Build one row for the letter, with a column for each letter in the target
            // word, plus one for the empty string at column 0
            foreach (var column in Enumerable.Range(1, columns - 1))
            {
                var insertCost = currentRow[column - 1] + 1;
                var deleteCost = previousRow[column] + 1;

                int replaceCost;
                if (word[column - 1] != letter)
                {
                    replaceCost = previousRow[column - 1] + 1;
                }
                else
                {
                    replaceCost = previousRow[column - 1];
                }

                currentRow.Add(Math.Min(Math.Min(insertCost, deleteCost), replaceCost));
            }

            // if the last entry in the row indicates the optimal cost is less than the
            // maximum cost, and there is a word in this trie node, then add it.
            if (currentRow.Last() <= maxDistance && node.Word != null)
            {
                result.Add(node);
            }

            // if any entries in the row are less than the maximum cost, then
            // recursively search each branch of the trie
            if (currentRow.Any(i => i <= maxDistance))
            {
                foreach (var child in node.Children)
                {
                    Find(child, char.ToLowerInvariant(child.Letter), word.ToLowerInvariant(), currentRow, result, maxDistance);
                }
            }
        }
    }
}
