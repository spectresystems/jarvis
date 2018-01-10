using System;
using Jarvis.Addin.StackExchange.Common;
using Xunit;

namespace Jarvis.Tests.Unit.Addins.StackExchange
{
    public class QuestionDescriptionFactoryTests
    {
        [Fact]
        public void Should_Resolve_To_Years_If_Created_One_Year_Ago()
        {
            // Given
            var created = DateTime.Now.AddYears(1);

            // When
            var friendlyDate = QuestionDescriptionFactory.GetUserFriendlyDate(created);

            // Then
            Assert.Equal("1 years ago", friendlyDate);
        }

        [Fact]
        public void Should_Use_Months_When_Less_Than_Two_Years_Ago()
        {
            // Given
            var created = DateTime.Now.AddYears(-1).AddMonths(-7);

            // When
            var friendlyDate = QuestionDescriptionFactory.GetUserFriendlyDate(created);

            // Then
            Assert.Equal("19 months ago", friendlyDate);
        }

        [Fact]
        public void Should_Resolve_To_Years_If_Created_More_Than_One_Year_Ago()
        {
            // Given
            var created = DateTime.Now.AddYears(-10);

            // When
            var friendlyDate = QuestionDescriptionFactory.GetUserFriendlyDate(created);

            // Then
            Assert.Equal("10 years ago", friendlyDate);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(365 * 1.5, 365, 2)]
        [InlineData(365 * 1.4, 365, 1)]
        public void Should_Approximate_As_Expected(int a, int b, int expected)
        {
            var result = QuestionDescriptionFactory.ApproximateDivition(a, b);
            Assert.Equal(expected, result);
        }
    }
}
