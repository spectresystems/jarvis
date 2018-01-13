using System;
using System.Linq;
using System.Text;
using Jarvis.Core;

namespace Jarvis.Addin.StackExchange.Common
{
    public interface IQuestionDescriptionFactory
    {
        string Create(Question question);
    }

    public class QuestionDescriptionFactory : IQuestionDescriptionFactory
    {
        private const string AnonymousUser = "anonymous";

        public string Create(Question question)
        {
            Ensure.NotNull(question, nameof(question));

            var description = new StringBuilder();

            description.Append($"Asked {GetUserFriendlyDate(question.CreationDate)}");
            if (string.Equals(question.Owner.DisplayName, AnonymousUser, StringComparison.CurrentCultureIgnoreCase))
            {
                description.Append($"by {question.Owner.DisplayName}");
            }
            description.Append(". ");

            description.Append($"{question.AnswerCount} answer");
            if (question.AnswerCount != 1)
            {
                description.Append("s");
            }
            if (question.AnswerCount != 0)
            {
                description.Append(question.IsAnswered ? ", including one " : ", none ");
                description.Append("marked as correct");
            }
            description.Append(". ");

            description.Append($"{question.Tags.Count} tag");
            if (question.Tags.Count != 1)
            {
                description.Append("s");
            }
            if (question.Tags.Any())
            {
                description.Append(", including ");

                var firstTag = question.Tags.FirstOrDefault();
                var secondTag = question.Tags.Skip(1).FirstOrDefault();
                var thridTag = question.Tags.Skip(2).FirstOrDefault();

                description.Append($"'{firstTag}'");
                if (!string.IsNullOrEmpty(secondTag))
                {
                    description.Append($", '{secondTag}'");
                }
                if (!string.IsNullOrEmpty(thridTag))
                {
                    description.Append($" and '{thridTag}'");
                }
                description.Append(".");
            }

            return description.ToString();
        }

        internal static string GetUserFriendlyDate(DateTime date)
        {
            var timeSinceAsked = DateTime.Now - date;

            var monthsAgo = ApproximateDivition(timeSinceAsked.Days,30);
            if (monthsAgo == 1)
            {
                return $"{monthsAgo} month ago";
            }
            if (monthsAgo > 1)
            {
                if (monthsAgo > 23)
                {
                    var yearsSinceAsked = ApproximateDivition(timeSinceAsked.Days, 365);
                    if (yearsSinceAsked == 1)
                    {
                        return $"{yearsSinceAsked} year ago";
                    }
                    if (yearsSinceAsked > 1)
                    {
                        return $"{yearsSinceAsked} years ago";
                    }
                }
                return $"{monthsAgo} months ago";
            }

            var weeksAgo = ApproximateDivition(timeSinceAsked.Days, 7);
            if (weeksAgo == 1)
            {
                return $"{weeksAgo} week ago";
            }
            if (weeksAgo > 1)
            {
                return $"{weeksAgo} weeks ago";
            }

            var daysAgo = timeSinceAsked.Days;
            if (daysAgo == 1)
            {
                return $"{daysAgo} day ago";
            }
            if (daysAgo > 1)
            {
                return $"{daysAgo} days ago";
            }

            var hoursAgo = timeSinceAsked.Hours;
            if (hoursAgo == 1)
            {
                return $"{hoursAgo} hour ago";
            }
            if (hoursAgo > 1)
            {
                return $"{hoursAgo} hours ago";
            }

            var minutesAgo = timeSinceAsked.Minutes;
            if (minutesAgo == 1)
            {
                return $"{minutesAgo} minute ago";
            }
            if (minutesAgo > 1)
            {
                return $"{minutesAgo} minutes ago";
            }

            return "just now";
        }

        internal static int ApproximateDivition(int a, int b)
        {
            var actualResult = a / (double) b;
            var roundedResult = Math.Round(actualResult, MidpointRounding.AwayFromZero);
            return Convert.ToInt32(roundedResult);
        }
    }
}
