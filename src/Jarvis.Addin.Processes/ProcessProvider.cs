using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Addin.Processes;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Interop;
using Jarvis.Core.Scoring;
using JetBrains.Annotations;

namespace Jarvis.Addin.Processes
{
    [UsedImplicitly]
    internal sealed class ProcessProvider : QueryProvider<ProcessResult>
    {
        private readonly IJarvisLog _log;

        public ProcessProvider(IJarvisLog log)
        {
            _log = log;
        }
        
        protected override Task<ImageSource> GetIconAsync(ProcessResult result)
        {
            return Task.FromResult(null as ImageSource);
        }

        public override Task<IEnumerable<IQueryResult>> QueryAsync(Query query)
        {
            return Task.Run(() =>
            {
                return (IEnumerable<IQueryResult>) Process.GetProcesses()
                    .Where(process => process.MainWindowTitle
                                          .IndexOf(query.Raw, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(process => (IQueryResult) new ProcessResult(
                        process.Id, process.MainWindowTitle, process.ProcessName,
                        LevenshteinScorer.Score(process.MainWindowTitle, query.Raw, false),
                        LevenshteinScorer.Score(process.MainWindowTitle, query.Raw)))
                    .ToList(); // Make LINQ execute inside Task before returning
            });
        }

        protected override Task ExecuteAsync(ProcessResult result)
        {
            var process = Process.GetProcessById(result.ProcessId);
            Win32.Window.SetForegroundWindow(process.MainWindowHandle);
                        
            if (Win32.Window.IsIconic(process.MainWindowHandle)) // Window is minimized
            {
                Win32.Window.ShowWindow(process.MainWindowHandle, 1);
            }

            return Task.CompletedTask;
        }
    }
}