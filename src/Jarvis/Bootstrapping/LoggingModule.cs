﻿// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Net;
using Autofac;
using Autofac.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Infrastructure.Diagnostics;
using Serilog;
using Serilog.Core;

#if !DEBUG || FAKERELEASE
using Jarvis.Infrastructure.Utilities;
using Spectre.System.IO;
#endif

namespace Jarvis.Bootstrapping
{
    public sealed class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Destructure.ByTransforming<HttpStatusCode>(code => $"{code} ({(int)code}")
                .Enrich.FromLogContext()
#if DEBUG && !FAKERELEASE

                .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss}][{Level}][{SourceContext}] {Message}{NewLine}{Exception}")

#else
                .WriteTo.File(
                    PathUtility.LogPath.CombineWithFilePath(new FilePath("log.txt")).FullPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
#endif
                .CreateLogger();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            // Ignore components that provide loggers
            if (registration.Services.OfType<TypedService>().Any(ts => ts.ServiceType == typeof(IJarvisLog)))
            {
                return;
            }

            registration.Preparing += (sender, args) =>
            {
                var serilogLogger = Log.ForContext(
                    Constants.SourceContextPropertyName, 
                    registration.Activator.LimitType.Name
                );
                IJarvisLog jarvisLogger = new SerilogLog(serilogLogger);
                args.Parameters = new[] { TypedParameter.From(jarvisLogger) }.Concat(args.Parameters);
            };
        }
    }
}
