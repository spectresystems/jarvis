// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Threading;
using Jarvis.Messages;
using Jarvis.Services.Updating;

namespace Jarvis.Services
{
    public sealed class UpdateService : IBackgroundWorker
    {
        private readonly IUpdateChecker _checker;
        private readonly ISettingsStore _settings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IJarvisLog _log;

        public string Name => "Update service";

        public UpdateService(IUpdateChecker checker, ISettingsStore settings,
            IEventAggregator eventAggregator, IJarvisLog log)
        {
            _checker = checker;
            _settings = settings;
            _eventAggregator = eventAggregator;
            _log = new LogDecorator("UpdateService", log);
        }

        public async Task<bool> Run(CancellationToken token)
        {
#if !DEBUG
            // Wait a minute after the application starts to check for updates.
            if (token.WaitHandle.WaitOne((int)TimeSpan.FromSeconds(15).TotalMilliseconds))
            {
                _log.Information("We were instructed to stop (1).");
                return true;
            }
#endif

            // Should we even check for updates?
            if (!_settings.Get<bool>(Constants.Settings.General.CheckForUpdates))
            {
                _log.Information("We were instructed to stop since we should not check for updates.");
                return true;
            }

            while (true)
            {
                _log.Information("Checking for updates...");
                var result = await _checker.CheckForUpdates();
                if (result != null && result.FutureVersion > result.CurrentVersion)
                {
                    _log.Information($"New version available: {result.FutureVersion}");
                    _eventAggregator.PublishOnUIThread(new UpdateAvailableMessage(result));

                    // Don't run this check again.
                    // At least not until we restart.
                    _log.Information("Shutting down since update message have been sent.");
                    return true;
                }

                // Sleep for 30 minutes before checking for updates again.
                _log.Information("No new version available.");
                if (token.WaitHandle.WaitOne((int)TimeSpan.FromMinutes(30).TotalMilliseconds))
                {
                    _log.Information("We were instructed to stop (2).");
                    return true;
                }
            }
        }
    }
}
