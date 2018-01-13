// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Caliburn.Micro;
using Jarvis.Core;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.ViewModels
{
    internal abstract class SelectFolderViewModel : Conductor<DirectoryPath>.Collection.OneActive
    {
        public bool CanRemoveFolder => ActiveItem != null;
        protected abstract string Description { get; }
        public bool IsDirty { get; private set; }

        public void Toggle(DirectoryPath item)
        {
            NotifyOfPropertyChange(nameof(CanRemoveFolder));
        }

        public void AddFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = Description;

                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Items.Add(new DirectoryPath(dialog.SelectedPath));
                    IsDirty = true;
                }
            }
        }

        public void RemoveFolder()
        {
            if (ActiveItem != null)
            {
                Items.Remove(ActiveItem);
                ActiveItem = null;
                NotifyOfPropertyChange(nameof(CanRemoveFolder));
                IsDirty = true;
            }
        }

        public abstract ValidationResult Validate();
        public abstract void Load(FileSettings settings);
        public abstract void Populate(ref FileSettings settings);
    }
}