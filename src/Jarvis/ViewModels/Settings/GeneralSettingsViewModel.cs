// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;
using Jarvis.Core;

namespace Jarvis.ViewModels.Settings
{
    public sealed class GeneralSettingsViewModel : Screen, ISettings
    {
        public string Name => "General";

        public bool CheckForUpdates { get; set; }
        public bool IncludePreviews { get; set; }

        public ValidationResult Validate()
        {
            return ValidationResult.Ok();
        }

        public void Load(ISettingsStore settings)
        {
            CheckForUpdates = settings.Get<bool>(Constants.Settings.General.CheckForUpdates);
            IncludePreviews = settings.Get<bool>(Constants.Settings.General.IncludePreviews);
        }

        public void Save(ISettingsStore settings)
        {
            settings.Set(Constants.Settings.General.CheckForUpdates, CheckForUpdates);
            settings.Set(Constants.Settings.General.IncludePreviews, IncludePreviews);
        }

        public void OnSaved()
        {
        }
    }
}
