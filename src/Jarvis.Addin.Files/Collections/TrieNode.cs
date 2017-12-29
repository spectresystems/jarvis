// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Jarvis.Addin.Files.Collections
{
    internal sealed class TrieNode<T>
    {
        public char Letter { get; }
        public IList<TrieNode<T>> Children { get; }
        public TrieNode<T> Parent { get; }
        public string Word { get; internal set; }
        public HashSet<T> Data { get; }

        internal TrieNode(TrieNode<T> parent, char letter)
        {
            Parent = parent;
            Children = new List<TrieNode<T>>();
            Word = null;
            Data = new HashSet<T>();
            Letter = letter;
        }

        public TrieNode<T> FindNode(char letter)
        {
            return Children?.FirstOrDefault(child => child.Letter == letter);
        }
    }
}