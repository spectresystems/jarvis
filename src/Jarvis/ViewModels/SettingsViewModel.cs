// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Autofac;
using Caliburn.Micro;
using Jarvis.Core;
using Jarvis.Services;
using Jarvis.ViewModels.Settings;
using IQueryProvider = Jarvis.Core.IQueryProvider;

namespace Jarvis.ViewModels
{
    public sealed class SettingsViewModel : Conductor<ISettings>.Collection.OneActive
    {
        private readonly SettingsService _settings;

        public delegate SettingsViewModel Factory();

        public string ValidationMessage { get; set; }

        public SettingsViewModel(IEnumerable<IQueryProvider> providers, SettingsService settings, ILifetimeScope scope)
        {
            _settings = settings;
            Items.Add(new GeneralSettingsViewModel());

            foreach (var provider in providers)
            {
                if (provider.SettingsType != null)
                {
                    if (scope.Resolve(provider.SettingsType) is ISettings item)
                    {
                        Items.Add(item);
                    }
                }
            }

            foreach (var item in Items)
            {
                item.Load(settings);
            }

            Toggle(Items.FirstOrDefault());
        }

        public void Toggle(ISettings settings)
        {
            if (settings != null)
            {
                ActivateItem(settings);
            }
        }

        public void Close()
        {
            TryClose(false);
        }

        public void Save()
        {
            // Validate everythig.
            foreach (var item in Items)
            {
                var result = item.Validate();
                if (result.HasError)
                {
                    ValidationMessage = result.Message;
                    NotifyOfPropertyChange(nameof(ValidationMessage));
                    return;
                }
            }

            // Save each screen.
            foreach (var item in Items)
            {
                item.Save(_settings);
            }

            // Save the settings.
            _settings.Save();
            foreach (var item in Items)
            {
                item.OnSaved();
            }

            // Close the dialog.
            TryClose(true);
        }
    }
}
