// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Autofac;
using Jarvis.Services.Updating;

namespace Jarvis.Infrastructure.Bootstrapping
{
    public sealed class UpdaterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
#if !DEBUG || FAKERELEASE
            builder.RegisterType<UpdateChecker>().As<IUpdateChecker>().SingleInstance();
#else
#if HAS_UPDATE
            builder.RegisterType<FakeUpdateChecker.WithUpdate>().As<IUpdateChecker>().SingleInstance();
#else
            builder.RegisterType<FakeUpdateChecker.WithoutUpdate>().As<IUpdateChecker>().SingleInstance();
#endif
#endif
        }
    }
}
