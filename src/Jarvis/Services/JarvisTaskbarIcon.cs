// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Hardcodet.Wpf.TaskbarNotification;
using Jarvis.ViewModels;
using JetBrains.Annotations;

namespace Jarvis.Services
{
    [UsedImplicitly]
    public sealed class JarvisTaskbarIcon : IDisposable
    {
        private readonly TaskbarIcon _icon;

        public JarvisTaskbarIcon(App application, TaskbarIconViewModel viewModel)
        {
            _icon = (TaskbarIcon)application.FindResource("SystemTrayIcon");
            if (_icon != null)
            {
                _icon.DataContext = viewModel;
            }
        }

        public void Dispose()
        {
            _icon?.Dispose();
        }
    }
}
