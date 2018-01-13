using System;
using System.Collections.Generic;

namespace Jarvis.Addin.StackExchange.Common
{
    public class Question
    {
        public IList<string> Tags { get; set; }
        public User Owner { get; set; }
        public bool IsAnswered { get; set; }
        public long ViewCount { get; set; }
        public long AnswerCount { get; set; }
        public long Score { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime CreationDate { get; set; }
        public long QuestionId { get; set; }
        public Uri Link { get; set; }
        public string Title { get; set; }

        public class User
        {
            public long Reputation { get; set; }
            public long UserId { get; set; }
            public string UserType { get; set; }
            public ushort AcceptRate { get; set; }
            public Uri ProfileImage { get; set; }
            public string DisplayName { get; set; }
            public Uri Link { get; set; }
        }
    }
}
