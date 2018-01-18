// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;
using Jarvis.Core;

namespace Jarvis.Addin.Files.ViewModels
{
    internal abstract class SelectPatternViewModel : Conductor<string>.Collection.OneActive
    {
        private string _pattern;

        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                NotifyOfPropertyChange(nameof(Pattern));
                NotifyOfPropertyChange(nameof(CanAddPattern));
                NotifyOfPropertyChange(nameof(CanRemovePattern));
            }
        }

        public bool CanRemovePattern => ActiveItem != null && Items.Contains(ActiveItem);
        public bool CanAddPattern
        {
            get
            {
                var processedPattern = Process(Pattern);
                return IsValid(processedPattern) && !Items.Contains(processedPattern);
            }
        }

        public bool IsDirty { get; private set; }

        public void Toggle(string item)
        {
            NotifyOfPropertyChange(nameof(CanRemovePattern));
        }

        public void AddPattern()
        {
            if (CanAddPattern)
            {
                Items.Add(Process(Pattern));
                Pattern = string.Empty;
                IsDirty = true;
            }
        }

        public void RemovePattern()
        {
            if (CanRemovePattern)
            {
                Items.Remove(ActiveItem);
                NotifyOfPropertyChange(nameof(CanRemovePattern));
                IsDirty = true;
            }
        }

        public abstract ValidationResult Validate();
        public abstract string Process(string value);
        public abstract bool IsValid(string value);
        public abstract void Load(FileSettings settings);
        public abstract void Populate(ref FileSettings settings);
    }
}