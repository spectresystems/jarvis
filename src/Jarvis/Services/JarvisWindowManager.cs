// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;
using Jarvis.Services.Updating;
using Jarvis.ViewModels;
using JetBrains.Annotations;

namespace Jarvis.Services
{
    [UsedImplicitly]
    public sealed class JarvisWindowManager : WindowManager
    {
        private readonly UpdateViewModel.Factory _updateFactory;
        private readonly AboutViewModel.Factory _aboutFactory;

        public JarvisWindowManager(
            UpdateViewModel.Factory updateFactory,
            AboutViewModel.Factory aboutFactory)
        {
            _updateFactory = updateFactory;
            _aboutFactory = aboutFactory;
        }

        public void ShowUpdateWindow(JarvisUpdateInfo info)
        {
            if (info != null)
            {
                Execute.OnUIThread(() =>
                {
                    ShowDialog(_updateFactory(info));
                });
            }
        }

        public void ShowAboutWindow()
        {
            Execute.OnUIThread(() =>
            {
                ShowDialog(_aboutFactory());
            });
        }
    }
}
