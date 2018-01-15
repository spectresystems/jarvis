
using System;
using System.Diagnostics;
using Jarvis.Core;
using JetBrains.Annotations;

namespace Jarvis.Addin.Windows
{
    [UsedImplicitly]
    [DebuggerDisplay("{" + nameof(Title) + ",nq}")]
    public class WindowsResult : IQueryResult, IEquatable<WindowsResult>
    {
    
        public IntPtr HWnd { get; }
        public Process Process { get; }
        public string Title { get; }
        public string Description { get; }
        public float Distance { get; }
        public float Score { get; }
        public QueryResultType Type => QueryResultType.Other;

        public WindowsResult(IntPtr hWnd, Process process, string title, string description, float distance, float score)
        {
            this.HWnd = hWnd;
            this.Process = process;
            this.Title = title;
            this.Description = description;
            this.Distance = distance;
            this.Score = score;
        }

        public bool Equals(IQueryResult obj)
        {
            return obj != null && obj.GetType() == GetType()
                   && Equals((WindowsResult)obj);
        }

        public bool Equals(WindowsResult other)
        {
            return other?.HWnd.Equals(HWnd) ?? false;
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == GetType()
                && Equals((WindowsResult)obj));
        }

        public override int GetHashCode()
        {
            return (int)HWnd;
        }
    }
}
