// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using Jarvis.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Infrastructure.Utilities;
using Spectre.System.IO;

namespace Jarvis.Services
{
    public sealed class SettingsService : ISettingsStore, IInitializable
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJarvisLog _log;
        private readonly object _lock;
        private readonly Dictionary<string, string> _settings;

        public SettingsService(IFileSystem fileSystem, IJarvisLog log)
        {
            _fileSystem = fileSystem;
            _log = log;
            _lock = new object();
            _settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        void IInitializable.Initialize()
        {
            Load();
        }

        public void Load()
        {
            lock (_lock)
            {
                _settings.Clear();

                var path = PathUtility.DataPath.CombineWithFilePath(new FilePath("settings.dat"));
                if (_fileSystem.Exist(path))
                {
                    try
                    {
                        using (var stream = _fileSystem.GetFile(path).OpenRead())
                        using (var reader = new BinaryReader(stream))
                        {
                            var count = reader.ReadInt32();
                            for (var index = 0; index < count; index++)
                            {
                                var key = reader.ReadString();
                                var value = reader.ReadString();
                                Set(key, value);
                            }
                        }

                        _log.Information($"Loaded settings from '{path.FullPath}'.");
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, $"An error occured while loading settings from '{path.FullPath}'.");
                    }
                }
                else
                {
                    // Set the default settings.
                    this.Set(Constants.Settings.General.CheckForUpdates, true);
                    this.Set(Constants.Settings.General.IncludePreviews, false);

                    // Save the default settings.
                    Save();
                }
            }
        }

        public void Save()
        {
            lock (_lock)
            {
                var path = PathUtility.DataPath.CombineWithFilePath(new FilePath("settings.dat"));

                try
                {
                    using (var stream = _fileSystem.GetFile(path).OpenWrite())
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(_settings.Count);
                        foreach (var setting in _settings)
                        {
                            writer.Write(setting.Key);
                            writer.Write(setting.Value);
                        }
                    }

                    _log.Information($"Saved settings to '{path.FullPath}'.");
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"An error occured while saving settings to '{path.FullPath}'.");
                }
            }
        }

        public string Get(string key)
        {
            lock (_lock)
            {
                _settings.TryGetValue(key, out var value);
                return value;
            }
        }

        public void Set(string key, string value)
        {
            lock (_lock)
            {
                _settings[key] = value;
            }
        }
    }
}
