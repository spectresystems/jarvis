using Jarvis.Core.Scoring;
using Xunit;

namespace Jarvis.Tests.Unit.Core.Scoring
{
    public sealed class LevenshteinScorerTests
    {
        [Theory]
        [InlineData("Google Chrome", "Goo", 10)]
        public void Should_Calculate_Levenshtein_Distance_Correctly(string source, string target, int expected)
        {
            // Given, When
            var result = LevenshteinScorer.Score(source, target, false);

            // Then
            Assert.Equal(expected, result);
        }
    }
}
