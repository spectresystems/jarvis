using System;
using System.Diagnostics;
using Jarvis.Core;
using JetBrains.Annotations;

namespace Jarvis.Addin.Processes
{
    [UsedImplicitly]
    [DebuggerDisplay("{" + nameof(Title) + ",nq}")]
    internal struct ProcessResult : IQueryResult, IEquatable<ProcessResult>
    {
        public int ProcessId { get; }
        public string Title { get; }
        public string Description { get; }
        public float Distance { get; }
        public float Score { get; }
        public QueryResultType Type => QueryResultType.Application;

        public ProcessResult(int processId, string title, string description,
            float distance, float score)
        {
            ProcessId = processId;
            Title = title;
            Description = description;
            Distance = distance;
            Score = score;
        }

        public bool Equals(IQueryResult obj)
        {
            return obj != null && obj.GetType() == GetType()
                               && Equals((ProcessResult) obj);
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == GetType()
                                   && Equals((ProcessResult) obj));
        }

        public bool Equals(ProcessResult other)
        {
            return other.ProcessId.Equals(ProcessId);
        }

        public override int GetHashCode()
        {
            return ProcessId.GetHashCode();
        }
    }
}