using System;
using System.Collections.Generic;

namespace Jarvis.Addin.StackExchange.Common
{
    public class Question
    {
        public IList<string> Tags { get; set; }
        public User Owner { get; set; }
        public bool IsAnswered { get; set; }
        public ulong ViewCount { get; set; }
        public ulong AnswerCount { get; set; }
        public ulong Score { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime CreationDate { get; set; }
        public ulong QuestionId { get; set; }
        public Uri Link { get; set; }
        public string Title { get; set; }

        public class User
        {
            public ulong Reputation { get; set; }
            public ulong UserId { get; set; }
            public string UserType { get; set; }
            public ushort AcceptRate { get; set; }
            public Uri ProfileImage { get; set; }
            public string DisplayName { get; set; }
            public Uri Link { get; set; }
        }
    }
}
