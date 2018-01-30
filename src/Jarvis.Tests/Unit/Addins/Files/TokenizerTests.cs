// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Jarvis.Addin.Files.Indexing;
using Shouldly;
using Xunit;

namespace Jarvis.Tests.Unit.Addins.Files
{
    public sealed class TokenizerTests
    {
        public sealed class TheTokenizePascalCaseMehthod
        {
            [Theory]
            [InlineData("Powershell", new[] { "Powershell" })]
            [InlineData("POWERSHELL", new[] { "POWERSHELL" })]
            [InlineData("pOwershell", new[] { "p", "Owershell" })]
            [InlineData("PowerShell", new[] { "Power", "Shell" })]
            [InlineData("powerShell", new[] { "power", "Shell" })]
            public void Should_Tokenize_Properly(string word, string[] parts)
            {
                // Given, When
                var result = Tokenizer.TokenizePascalCase(word).ToArray();

                // Then
                result.Length.ShouldBe(parts.Length);
                result.ShouldBe(parts);
            }
        }
    }
}
