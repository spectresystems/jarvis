// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Net;
using Autofac;
using Autofac.Core;
using Jarvis.Core.Diagnostics;
using Jarvis.Infrastructure.Diagnostics;
using Serilog;

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
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Destructure.ByTransforming<HttpStatusCode>(code => $"{code} ({(int)code}")
                .Enrich.FromLogContext();

#if DEBUG && !FAKERELEASE

            Serilog.Debugging.SelfLog.Enable(s => System.Diagnostics.Debug.WriteLine(s));

            loggerConfig
                .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss}][{Level}][{SourceContext}] {Message} {Properties}{NewLine}{Exception}");

#else

            loggerConfig
                .WriteTo.File(
                    PathUtility.LogPath.CombineWithFilePath(new FilePath("log.txt")).FullPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);

#endif
            Log.Logger = loggerConfig.CreateLogger();
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
                    Serilog.Core.Constants.SourceContextPropertyName,
                    registration.Activator.LimitType.Name);
                IJarvisLog jarvisLogger = new SerilogLog(serilogLogger);
                args.Parameters = new[] { TypedParameter.From(jarvisLogger) }.Concat(args.Parameters);
            };
        }
    }
}
