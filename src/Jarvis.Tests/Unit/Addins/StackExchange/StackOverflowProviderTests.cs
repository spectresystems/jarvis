using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Addin.StackExchange;
using Jarvis.Addin.StackExchange.Common;
using Jarvis.Addin.StackExchange.Common.QueryLanguage;
using Jarvis.Addin.StackExchange.StackOverflow;
using Jarvis.Core;
using NSubstitute;
using NSubstitute.Core.Arguments;
using Xunit;

namespace Jarvis.Tests.Unit.Addins.StackExchange
{
    public class StackOverflowProviderTests
    {
        [Fact]
        public async Task Should_Handle_Simple_Queries_As_Text_In_Question_Title()
        {
            // Given
            const string simpleQuery = "so pattern matching";

            var stackExchangeClient = Substitute.For<IStackExchangeClient>();
            var queryParser = new QueryParser<SearchQuery>();
            var provider = new StackOverflowProvider(stackExchangeClient, queryParser);

            stackExchangeClient
                .SearchAsync(Arg.Any<SearchQuery>(), CancellationToken.None)
                .Returns(Task.FromResult<IList<Question>>(new List<Question>()));

            var query = new Query(simpleQuery);

            // When
            await provider.QueryAsync(query);

            // Then
            var stackExchangeQuery = stackExchangeClient.ReceivedCalls().First().GetArguments()[0] as SearchQuery;
            Assert.NotNull(stackExchangeQuery);
            Assert.Equal(stackExchangeQuery.InTitle, "pattern matching");
        }

        [Fact]
        public async Task Should_Handle_Queries_With_Free_Text_And_Expressions()
        {
            // Given
            const string simpleQuery = "so pattern matching tagged:c#";

            var stackExchangeClient = Substitute.For<IStackExchangeClient>();
            var queryParser = new QueryParser<SearchQuery>();
            var provider = new StackOverflowProvider(stackExchangeClient, queryParser);

            stackExchangeClient
                .SearchAsync(Arg.Any<SearchQuery>(), CancellationToken.None)
                .Returns(Task.FromResult<IList<Question>>(new List<Question>()));

            var query = new Query(simpleQuery);

            // When
            await provider.QueryAsync(query);

            // Then
            var stackExchangeQuery = stackExchangeClient.ReceivedCalls().First().GetArguments()[0] as SearchQuery;
            Assert.NotNull(stackExchangeQuery);
            Assert.Equal(stackExchangeQuery.InTitle, "pattern matching");
            Assert.Contains("c#", stackExchangeQuery.Tagged);
        }
    }
}
