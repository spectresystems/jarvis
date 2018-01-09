using System;
using System.Linq;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.Common.QueryLanguage;
using Xunit;

namespace Jarvis.Tests.Unit.Addins.StackExchange
{
    public class QueryParserTests
    {
        [Fact]
        public void Should_Be_Able_To_Bind_String_Properties()
        {
            // Given
            const string siteName = "stackoverflow";
            var query = $"site:{siteName}";
            var parser = new QueryParser<SearchQuery>();

            // When
            var result = parser.Bind(query);

            // Then
            Assert.Equal(siteName, result.Site);
        }

        [Fact]
        public void Should_Be_Able_To_Bind_List_To_IEnumerable_Property()
        {
            // Given
            const string firstTag = "jarvis";
            const string secondTag = "alfred";
            const string thirdTag = "spotlight";
            var query = $"tagged:{firstTag} tagged:{secondTag} tagged:{thirdTag}";
            var parser = new QueryParser<SearchQuery>();

            // When
            var result = parser.Bind(query);

            // Then
            Assert.Equal(result.Tagged.Count(), 3);
            Assert.Contains(firstTag, result.Tagged);
            Assert.Contains(secondTag, result.Tagged);
            Assert.Contains(thirdTag, result.Tagged);
        }

        [Fact]
        public void Should_Be_Able_To_Bind_Strings_To_Enum_Property()
        {
            // Given
            var sortedByVotes = SearchQuery.SortType.Votes;
            var query = "sort:votes";
            var parser = new QueryParser<SearchQuery>();

            // When
            var result = parser.Bind(query);

            // Then
            Assert.Equal(result.Sort, sortedByVotes);
        }

        [Theory]
        [InlineData("2018-01-09")]
        [InlineData("2018/01/09")]
        public void Should_Be_Able_To_Bind_Date_String_To_DateTime(string dateString)
        {
            // Given
            var expectedDate = new DateTime(2018,01,09);
            var query = $"from:{dateString}";
            var parser = new QueryParser<SearchQuery>();

            // When
            var result = parser.Bind(query);

            // Then
            Assert.Equal(expectedDate, result.From);
        }

        [Fact]
        public void Should_Be_Able_To_Combine_Expressions()
        {
            // Given
            var query = "tagged:jarvis tagged:c# nottagged:alfred sort:votes from:2018-01-01";
            var parser = new QueryParser<SearchQuery>();

            // When
            var result = parser.Bind(query);

            // Then
            Assert.Contains("jarvis", result.Tagged);
            Assert.Contains("c#", result.Tagged);
            Assert.Contains("alfred", result.NotTagged);
            Assert.Equal(SearchQuery.SortType.Votes, result.Sort);
            Assert.Equal(new DateTime(2018,01,01), result.From);
        }

        [Fact]
        public void Should_Return_Unmatched_Expressions()
        {
            // Given
            var unmatchedKey = "followers";
            var unmatchedValue = "100";
            var query = $"{unmatchedKey}:{unmatchedValue}";
            var parser = new QueryParser<SearchQuery>();

            // When
            parser.Bind(query, out var unbound, out _);

            // Then
            Assert.Contains(unmatchedKey, unbound.Keys);
            Assert.Single(unbound[unmatchedKey]);
            Assert.Equal(unbound[unmatchedKey].First(), unmatchedValue);
        }

        [Fact]
        public void Should_Return_Non_Expression_Strings()
        {
            // Given
            var query = "pattern matching tagged:c#";
            var parser = new QueryParser<SearchQuery>();

            // When
            parser.Bind(query, out _, out var unmatched);

            // Then
            Assert.Equal(unmatched, "pattern matching");
        }
    }
}
