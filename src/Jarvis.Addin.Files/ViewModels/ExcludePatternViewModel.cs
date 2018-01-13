// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using Jarvis.Core;

namespace Jarvis.Addin.Files.ViewModels
{
    internal sealed class ExcludePatternViewModel : SelectPatternViewModel
    {
        public override ValidationResult Validate()
        {
            return ValidationResult.Ok();
        }

        public override string Process(string value)
        {
            return value?.Trim();
        }

        public override bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            var invalidCharacters = Path.GetInvalidPathChars();
            return value.All(character => !invalidCharacters.Contains(character));
        }

        public override void Load(FileSettings settings)
        {
            Items.AddRange(settings.ExcludedPatterns);
        }

        public override void Populate(ref FileSettings settings)
        {
            settings.ExcludedPatterns.Clear();
            settings.ExcludedPatterns.AddRange(Items);
        }
    }
}
