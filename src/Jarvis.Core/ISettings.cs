// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;

namespace Jarvis.Core
{
    public interface ISettings : IScreen
    {
        string Name { get; }

        ValidationResult Validate();

        void Load(ISettingsStore settings);
        void Save(ISettingsStore settings);

        void OnSaved();
    }
}
