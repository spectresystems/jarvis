// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using Caliburn.Micro;
using Jarvis.Infrastructure;
using Jarvis.Messages;
using Jarvis.Services;
using JetBrains.Annotations;

namespace Jarvis.ViewModels
{
    [UsedImplicitly]
    public sealed class TaskbarIconViewModel
    {
        private readonly WindowService _windowManager;
        private readonly IEventAggregator _eventAggregator;

        public TaskbarIconViewModel(
            WindowService windowManager,
            IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
        }

        [UsedImplicitly]
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand(
                  args => ShowJarvis(),
                  args => true);
            }
        }

        public void ShowJarvis()
        {
            _windowManager.ShowQueryWindow();
        }

        public void ShowSettings()
        {
            _windowManager.ShowSettingsWindow();
        }

        public void ShowAbout()
        {
            _windowManager.ShowAboutWindow();
        }

        [UsedImplicitly]
        public void ExitJarvis()
        {
            _eventAggregator.PublishOnCurrentThread(new ExitMessage());
        }
    }
}
