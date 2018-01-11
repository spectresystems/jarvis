// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Core;

namespace Jarvis.Addin.Files.ViewModels
{
    internal sealed class FileSettingsViewModel : Screen, ISettings
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _hasChanges;

        public string Name => "File indexing";

        public IncludeFolderViewModel IncludeFolders { get; }
        public IncludeExtensionsViewModel IncludeExtensions { get; }
        public ExcludeFolderViewModel ExcludeFolders { get; }
        public ExcludePatternViewModel ExcludePatterns { get; }

        public FileSettingsViewModel(
            IEventAggregator eventAggregator,
            IncludeFolderViewModel includeFolders,
            IncludeExtensionsViewModel includeExtensions,
            ExcludeFolderViewModel excludeFolders,
            ExcludePatternViewModel excludePatterns)
        {
            _eventAggregator = eventAggregator;
            IncludeFolders = includeFolders;
            IncludeExtensions = includeExtensions;
            ExcludeFolders = excludeFolders;
            ExcludePatterns = excludePatterns;
        }

        public ValidationResult Validate()
        {
            return ValidationResult.Ok();
        }

        public void Load(ISettingsStore settings)
        {
            var model = FileSettings.Load(settings);

            IncludeFolders.Load(model);
            IncludeExtensions.Load(model);
            ExcludeFolders.Load(model);
            ExcludePatterns.Load(model);
        }

        public void Save(ISettingsStore settings)
        {
            var model = new FileSettings();

            IncludeFolders.Populate(ref model);
            IncludeExtensions.Populate(ref model);
            ExcludeFolders.Populate(ref model);
            ExcludePatterns.Populate(ref model);

            model.Save(settings);

            if (!_hasChanges)
            {
                _hasChanges = IncludeFolders.IsDirty || IncludeExtensions.IsDirty
                    || ExcludeFolders.IsDirty || ExcludePatterns.IsDirty;
            }
        }

        public void OnSaved()
        {
            if (_hasChanges)
            {
                _eventAggregator.PublishOnCurrentThread(new TriggerIndexMessage());
            }
        }
    }
}
