// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Interop;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Sources.Uwp
{
    internal sealed class AppxManifestReader
    {
        private readonly IFileSystem _fileSystem;
        private readonly INativeStreamProvider _streamProvider;
        private readonly IJarvisLog _log;

        public AppxManifestReader(IFileSystem fileSystem, INativeStreamProvider streamProvider, IJarvisLog log)
        {
            _fileSystem = fileSystem;
            _streamProvider = streamProvider;
            _log = log;
        }

        public AppxManifest Read(FilePath path)
        {
            var root = path.GetDirectory();

            // Get the stream to the manifest file.
            var stream = _streamProvider.GetStream(path);
            if (stream == null)
            {
                _log.Error($"Could not get stream for '{path.FullPath}'.");
                return null;
            }

            try
            {
                var appx = new AppxPackagingInterop.AppxFactory() as AppxPackagingInterop.IAppxFactory;
                var reader = appx.CreateManifestReader(stream);

                var identity = reader.GetPackageId().GetPackageFullName();

                // Read manifest properties.
                var properties = reader.GetProperties();
                var manifest = new AppxManifest
                {
                    Id = reader.GetPackageId().GetName(),
                    DisplayName = ParseString(identity, () => properties.ReadString("DisplayName")),
                    Description = ParseString(identity, () => properties.ReadString("Description")),
                    Publisher = ParseString(identity, () => properties.ReadString("PublisherDisplayName")),
                    Logo = ParseImage(root, () => properties.ReadString("Logo"))
                };

                // Read information about applications.
                var applications = reader.GetApplications();
                while (applications.GetHasCurrent() != 0)
                {
                    var app = applications.GetCurrent();

                    // Create the application representation.
                    var application = new AppxManifestApplication
                    {
                        Id = ParseString(identity, () => app.ReadString("Id")),
                        AppUserModelId = app.GetAppUserModelId(),
                        ShortName = ParseString(identity, () => app.ReadString("ShortName")),
                        DisplayName = ParseString(identity, () => app.ReadString("DisplayName")),
                        Logo = ParseImage(root, () => app.ReadString("Logo")),
                        SmallLogo = ParseImage(root, () => app.ReadString("SmallLogo")),
                        BackgroundColor = ParseString(identity, () => app.ReadString("BackgroundColor") ?? "Transparent"),
                        EntryPoint = ParseString(identity, () => app.ReadString("EntryPoint")),
                        Executable = ParseString(identity, () => app.ReadString("Executable")),
                        StartPage = ParseString(identity, () => app.ReadString("StartPage"))
                    };

                    // Parse images.
                    ParseImage(root, () => app.ReadString("Square310x310Logo"), img => application.Images.Add(img));
                    ParseImage(root, () => app.ReadString("Square150x150Logo"), img => application.Images.Add(img));
                    ParseImage(root, () => app.ReadString("Square70x70Logo"), img => application.Images.Add(img));
                    ParseImage(root, () => app.ReadString("Square30x30Logo"), img => application.Images.Add(img));

                    manifest.Applications.Add(application);
                    applications.MoveNext();
                }

                return manifest;
            }
            finally
            {
                // Release the stream.
                _streamProvider.ReleaseStream(stream);
            }
        }

        private void ParseImage(DirectoryPath root, Func<string> func, Action<AppxManifestAsset> action)
        {
            var result = ParseImage(root, func);
            if (result != null)
            {
                action(result);
            }
        }

        private AppxManifestAsset ParseImage(DirectoryPath root, Func<string> func)
        {
            var resource = func();
            if (string.IsNullOrWhiteSpace(resource))
            {
                return null;
            }

            var file = root.CombineWithFilePath(new FilePath(resource));
            if (_fileSystem.Exist(file))
            {
                return new AppxManifestAsset
                {
                    Name = resource,
                    Path = file
                };
            }

            var stack = new Stack<string>(new[] { "400", "300", "200", "150", "125", "100" });
            while (stack.Count > 0)
            {
                var current = stack.Pop();

                var scale = file.RemoveExtension()
                    .AppendExtension(new FileExtension($"scale-{current}"))
                    .AppendExtension(file.GetExtension());

                if (_fileSystem.Exist(scale))
                {
                    return new AppxManifestAsset
                    {
                        Name = resource,
                        Path = scale
                    };
                }
            }

            return null;
        }

        private static string ParseString(string identity, Func<string> func)
        {
            var resource = func();

            if (string.IsNullOrWhiteSpace(resource))
            {
                return null;
            }

            if (IsPriResource(resource))
            {
                // Generate the source for the PRI resource.
                var key = ParsePriResourceKey(resource);
                var source = $"@{{{identity}? ms-resource:{key}}}";

                // Read the resource value.
                var builder = new StringBuilder(255);
                var result = Win32.SHLoadIndirectString(source, builder, (uint)builder.Capacity, IntPtr.Zero);
                if (result == Win32.Hresult.Ok)
                {
                    var resourceValue = builder.ToString();
                    if (!string.IsNullOrWhiteSpace(resourceValue))
                    {
                        return resourceValue;
                    }
                }
            }

            return resource;
        }

        private static bool IsPriResource(string value)
        {
            return value != null && value.StartsWith("ms-resource:");
        }

        private static string ParsePriResourceKey(string resource)
        {
            var key = resource.Replace("ms-resource:", string.Empty);
            if (key.StartsWith("//"))
            {
                return key;
            }
            if (key.StartsWith("/"))
            {
                return "//" + key;
            }
            return "///resources/" + key;
        }
    }
}