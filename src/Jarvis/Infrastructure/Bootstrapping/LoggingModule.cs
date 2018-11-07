// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Autofac;
using Jarvis.Core.Diagnostics;

#if !DEBUG || FAKERELEASE
using Jarvis.Infrastructure.Diagnostics;
using Jarvis.Infrastructure.Utilities;
using Serilog;
using Spectre.System.IO;
#endif

namespace Jarvis.Infrastructure.Bootstrapping
{
    public sealed class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
#if DEBUG && !FAKERELEASE
            builder.RegisterType<DebugLog>().As<IJarvisLog>().SingleInstance();
#else
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(
                    PathUtility.LogPath.CombineWithFilePath(new FilePath("log.txt")).FullPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
                .CreateLogger();

            builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
            builder.RegisterType<SerilogLog>().As<IJarvisLog>().SingleInstance();
#endif
        }
    }
}
