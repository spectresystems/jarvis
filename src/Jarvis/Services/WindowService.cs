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
    public sealed class WindowService : WindowManager
    {
        private readonly ApplicationService _application;
        private readonly UpdateViewModel.Factory _updateFactory;
        private readonly SettingsViewModel.Factory _settingsFactory;
        private readonly AboutViewModel.Factory _aboutFactory;

        public WindowService(
            ApplicationService application,
            UpdateViewModel.Factory updateFactory,
            SettingsViewModel.Factory settingsFactory,
            AboutViewModel.Factory aboutFactory)
        {
            _application = application;
            _updateFactory = updateFactory;
            _settingsFactory = settingsFactory;
            _aboutFactory = aboutFactory;
        }

        public void ShowQueryWindow()
        {
            _application.Show();
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

        public void ShowSettingsWindow()
        {
            Execute.OnUIThread(() =>
            {
                ShowDialog(_settingsFactory());
            });
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
