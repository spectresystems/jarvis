using Jarvis.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Scoring;

namespace Jarvis.Addin.Windows
{
    public class WindowsProvider : QueryProvider<WindowsResult>
    {
        private readonly IJarvisLog _log;

        public override string Command => "w";

        public WindowsProvider(IJarvisLog log)
        {
            _log = log;
        }

        public override Task<IEnumerable<IQueryResult>> QueryAsync(Query query)
        {
            return Task.Run<IEnumerable<IQueryResult>>(() =>
             {
                 Ensure.NotNull(query, nameof(query));
                 List<WindowsResult> results = new List<WindowsResult>();

                 bool enumWindows = WindowsImports.EnumWindows(delegate (IntPtr hWnd, int lParam)
                 {
                     if (!WindowsImports.IsWindowVisible(hWnd)) return true;

                     int length = WindowsImports.GetWindowTextLength(hWnd);
                     if (length == 0) return true;
                     IntPtr processHandle = WindowsImports.GetProcessHandleFromHwnd(hWnd);

                     StringBuilder builder = new StringBuilder(length);
                     WindowsImports.GetWindowText(hWnd, builder, length + 1);
                     var title = builder.ToString();
                     var processName = "";

                     try
                     {
                         StringBuilder filePath = new StringBuilder(2000);
                         WindowsImports.GetProcessImageFileName(processHandle, filePath, 2000);
                         processName = Path.GetFileName(filePath.ToString());
                     }
                     catch (Exception ex)
                     {
                         // ignore
                     }

                     // TODO not best string compare algorithm for this case
                     results.Add(new WindowsResult(hWnd, null, title, processName,
                         Math.Min(LevenshteinScorer.Score(processName, query.Argument, false),
                            LevenshteinScorer.Score(title, query.Argument, false)),
                         Math.Min(LevenshteinScorer.Score(processName, query.Argument),
                             LevenshteinScorer.Score(title, query.Argument))));

                     return true;
                 }, 0);

                 if (!enumWindows)
                 {
                     _log.Error(new Exception("Windows enum failed"), "Windows enumeration failed");
                     return Enumerable.Empty<IQueryResult>();
                 }

                 return results.OrderBy(result => result.Score).Take(5);

             });
        }

        protected override Task ExecuteAsync(WindowsResult result)
        {
            WindowsImports.BringWindowToFront(result.HWnd);
            return Task.CompletedTask;
        }

        protected override async Task<ImageSource> GetIconAsync(WindowsResult result) => ImageUtils.GetImageStream(WindowsImports.GetWindowIcon(result.HWnd));
        
    }
}