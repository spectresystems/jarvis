// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Jarvis.Messages;
using Jarvis.Services;
using Jarvis.Services.Updating;
using JetBrains.Annotations;

namespace Jarvis.ViewModels
{
    public sealed class ShellViewModel : Screen, IHandle<UpdateAvailableMessage>, IHandle<ExitMessage>
    {
        private readonly ApplicationService _application;
        private readonly WindowService _windowManager;
        private readonly Subject<string> _queryStream;

        [UsedImplicitly]
        public string QueryString { get; set; }
        [UsedImplicitly]
        public ResultViewModel Result { get; }
        [UsedImplicitly]
        public JarvisUpdateInfo UpdateInfo { get; set; }
        [UsedImplicitly]
        public Visibility UpdateAvailableVisibility => UpdateInfo != null ? Visibility.Visible : Visibility.Collapsed;

        public ShellViewModel(
            ResultViewModel resultViewModel, ApplicationService application,
            WindowService windowManager, IEventAggregator inbox)
        {
            _application = application;
            _windowManager = windowManager;
            _queryStream = new Subject<string>();
            SetupQueryStream();
            Result = resultViewModel;

            // Subscribe to messages.
            inbox.Subscribe(this);
        }

        private void SetupQueryStream()
        {
            _queryStream
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(async q =>
                {
                    QueryString = q;
                    await Result.ExecuteQuery(q);
                });
        }

        [UsedImplicitly]
        public void ExecuteQuery(string queryString) => _queryStream.OnNext(queryString);

        [UsedImplicitly]
        public async Task OnKeyDown(KeyEventArgs keyArgs)
        {
            if (keyArgs.Key == Key.Enter)
            {
                await Result.ExecuteSelected();
            }
            else if (keyArgs.Key == Key.Escape)
            {
                _application.Hide();
            }
            else if (keyArgs.Key == Key.Down)
            {
                Result.MoveSelection(ResultViewModel.Selection.Next);
            }
            else if (keyArgs.Key == Key.Up)
            {
                Result.MoveSelection(ResultViewModel.Selection.Previous);
            }
            else if (keyArgs.Key == Key.Next)
            {
                Result.MoveSelection(ResultViewModel.Selection.Last);
            }
            else if (keyArgs.Key == Key.PageUp)
            {
                Result.MoveSelection(ResultViewModel.Selection.First);
            }
        }

        [UsedImplicitly]
        public void OnKeyboardFocus(TextBox textbox)
        {
            textbox.Select(0, textbox.Text.Length);
        }

        [UsedImplicitly]
        public void OnUpdateAvailableClicked()
        {
            _windowManager.ShowUpdateWindow(UpdateInfo);
            Handle(new UpdateAvailableMessage(null));
        }

        [UsedImplicitly]
        public void OnDeactivated()
        {
            _application.Hide();
        }

        [UsedImplicitly]
        public void OnShowAbout()
        {
            _windowManager.ShowAboutWindow();
        }

        [UsedImplicitly]
        public void OnClose(CancelEventArgs e)
        {
            if (!_application.IsWindowsShuttingDown())
            {
                var result = MessageBox.Show("Close Jarvis?", "Jarvis", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        public void Handle(UpdateAvailableMessage message)
        {
            UpdateInfo = message.UpdateInfo;
            NotifyOfPropertyChange(nameof(UpdateAvailableVisibility));
        }

        public void Handle(ExitMessage message)
        {
            _application.Quit();
        }
    }
}