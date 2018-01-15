// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using Jarvis.Infrastructure.Utilities;
using Jarvis.Services.Updating;
using JetBrains.Annotations;
using Spectre.System.IO;

namespace Jarvis.ViewModels
{
    public sealed class UpdateViewModel : Screen
    {
        private readonly JarvisUpdateInfo _update;
        private readonly IFileSystem _fileSystem;
        private bool _isInstalling;

        public delegate UpdateViewModel Factory(JarvisUpdateInfo info);

        [UsedImplicitly]
        public string Version => _update.FutureVersion.ToString();
        [UsedImplicitly]
        public string Url => _update.ReleaseUri.AbsoluteUri;

        [UsedImplicitly]
        public bool CanInstall => !IsInstalling && _update.InstallerUri != null;
        [UsedImplicitly]
        public bool CanCloseDialog => !IsInstalling;

        [UsedImplicitly]
        public bool IsInstalling
        {
            get => _isInstalling;
            set
            {
                _isInstalling = value;
                NotifyOfPropertyChange(nameof(IsInstalling));
                NotifyOfPropertyChange(nameof(CanInstall));
                NotifyOfPropertyChange(nameof(CanCloseDialog));
            }
        }

        public UpdateViewModel(JarvisUpdateInfo info, IFileSystem fileSystem)
        {
            _update = info;
            _fileSystem = fileSystem;
        }

        [UsedImplicitly]
        public void OpenBrowser()
        {
            Process.Start(Url);
        }

        [UsedImplicitly]
        public async Task Install()
        {
            try
            {
                // Get the installer path.
                var installerPath = PathUtility.InstallerPath.CombineWithFilePath(new FilePath(_update.InstallerName));
                if (!_fileSystem.Exist(installerPath))
                {
                    IsInstalling = true;

                    // Download asset.
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(_update.InstallerUri);
                        if (response.IsSuccessStatusCode)
                        {
                            using (var contentStream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(installerPath.FullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                            {
                                await contentStream.CopyToAsync(fileStream);
                            }
                        }
                    }
                }

                // Launch the installer.
                Process.Start(installerPath.FullPath);

                // Close the dialog.
                TryClose(true);
            }
            finally
            {
                IsInstalling = false;
            }
        }

        [UsedImplicitly]
        public void CloseDialog()
        {
            TryClose(false);
        }
    }
}
