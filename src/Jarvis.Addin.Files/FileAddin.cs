// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Autofac;
using Jarvis.Addin.Files.Icons;
using Jarvis.Addin.Files.Indexing;
using Jarvis.Addin.Files.Sources;
using Jarvis.Addin.Files.Sources.Uwp;
using Jarvis.Core;
using Jarvis.Core.Threading;

namespace Jarvis.Addin.Files
{
    public sealed class FileAddin : IAddin
    {
        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<FileProvider>().As<IQueryProvider>().SingleInstance();
            builder.RegisterType<FileIndexer>().As<IBackgroundWorker>().As<IFileIndex>().SingleInstance();
            builder.RegisterType<FileSettingsSeeder>().As<ISettingsSeeder>().SingleInstance();

            // Sources
            builder.RegisterType<StartMenuIndexSource>().As<IFileIndexSource>().SingleInstance();
            builder.RegisterType<UwpIndexSource>().As<IFileIndexSource>().SingleInstance();
            builder.RegisterType<DocumentIndexSource>().As<IFileIndexSource>().SingleInstance();

            // Utilities
            builder.RegisterType<AppxManifestReader>().SingleInstance();
            builder.RegisterType<IconLoader>().SingleInstance();
            builder.RegisterType<NativeStreamProvider>().As<INativeStreamProvider>().SingleInstance();
        }
    }
}
