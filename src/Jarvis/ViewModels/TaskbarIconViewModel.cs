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
        private readonly ApplicationService _application;
        private readonly IEventAggregator _eventAggregator;

        public TaskbarIconViewModel(ApplicationService application, IEventAggregator eventAggregator)
        {
            _application = application;
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
            _application.Show();
        }

        [UsedImplicitly]
        public void ExitJarvis()
        {
            _eventAggregator.PublishOnCurrentThread(new ExitMessage());
        }
    }
}
