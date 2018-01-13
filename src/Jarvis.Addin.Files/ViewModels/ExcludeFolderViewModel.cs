// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Jarvis.Core;

namespace Jarvis.Addin.Files.ViewModels
{
    internal sealed class ExcludeFolderViewModel : SelectFolderViewModel
    {
        protected override string Description => "Select a folder to exclude from indexing.";

        public override ValidationResult Validate()
        {
            return ValidationResult.Ok();
        }

        public override void Load(FileSettings settings)
        {
            Items.AddRange(settings.ExcludedFolders);
        }

        public override void Populate(ref FileSettings settings)
        {
            settings.ExcludedFolders.Clear();
            settings.ExcludedFolders.AddRange(Items);
        }
    }
}
