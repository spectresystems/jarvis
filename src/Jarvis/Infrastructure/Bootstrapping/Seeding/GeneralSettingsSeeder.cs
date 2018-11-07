// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Jarvis.Core;

namespace Jarvis.Infrastructure.Bootstrapping.Seeding
{
    public sealed class GeneralSettingsSeeder : ISettingsSeeder
    {
        public void Seed(ISettingsStore store)
        {
            // Check for updates?
            if (!store.Exist(Constants.Settings.General.CheckForUpdates))
            {
                store.Set(Constants.Settings.General.CheckForUpdates, true);
            }

            // Include previews?
            if (!store.Exist(Constants.Settings.General.IncludePreviews))
            {
                store.Set(Constants.Settings.General.IncludePreviews, false);
            }
        }
    }
}
