// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Caliburn.Micro;
using Jarvis.Services.Updating;
using JetBrains.Annotations;

namespace Jarvis.ViewModels
{
    public sealed class UpdateViewModel : Screen
    {
        private readonly JarvisUpdateInfo _update;

        public delegate UpdateViewModel Factory(JarvisUpdateInfo info);

        [UsedImplicitly]
        public string Version => _update.FutureVersion.ToString();
        [UsedImplicitly]
        public string Url => _update.Uri.AbsoluteUri;

        public UpdateViewModel(JarvisUpdateInfo info)
        {
            _update = info;
        }

        [UsedImplicitly]
        public void OpenBrowser()
        {
            Process.Start(Url);
            TryClose(true);
        }

        [UsedImplicitly]
        public void CloseDialog()
        {
            TryClose(false);
        }
    }
}
