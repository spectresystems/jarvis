// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Infrastructure.Input;

namespace Jarvis.Services
{
    public sealed class KeyboardService : IInitializable, IDisposable
    {
        private readonly ApplicationService _applicationService;
        private readonly IJarvisLog _log;
        private IKeyboardHook _hook;

        public KeyboardService(ApplicationService applicationService, IJarvisLog log)
        {
            _applicationService = applicationService;
            _log = new LogDecorator(nameof(KeyboardService), log);
        }

        public void Dispose()
        {
            _hook?.Dispose();
        }

        public void Initialize()
        {
            try
            {
                _log.Information("Registering Windows hot key...");
                _hook = new HotKeyKeyboardHook(() => _applicationService.Toggle());
                _hook.Register();
                _log.Information("Hot key was successfully registered.");
            }
            catch (Exception ex)
            {
                _log.Error($"An error occured when registering hot key: {ex.Message}");

                try
                {
                    _log.Information("Registering windows keyboard hook...");
                    _hook = new GlobalKeyboardHook(() => _applicationService.Toggle());
                    _hook.Register();
                    _log.Information("Keyboard hook was successfully registered.");
                }
                catch (Exception ex2)
                {
                    // TODO: Notify the user that registering wasn't possible.
                    _log.Error($"Unable to register windows keyboard hook: {ex2.Message}");
                }
            }
        }
    }
}
