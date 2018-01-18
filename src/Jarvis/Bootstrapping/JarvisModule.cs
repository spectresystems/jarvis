// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using Autofac;
using Caliburn.Micro;
using Jarvis.Bootstrapping.Seeding;
using Jarvis.Core;
using Jarvis.Core.Threading;
using Jarvis.Services;
using Jarvis.ViewModels;
using Spectre.System.IO;
using Module = Autofac.Module;

namespace Jarvis.Bootstrapping
{
    public sealed class JarvisModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Caliburn.Micro
            builder.RegisterType<WindowService>().As<IWindowManager>().As<WindowService>().InstancePerLifetimeScope();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();

            // Core
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();

            // Background services.
            builder.RegisterType<ServiceOrchestrator>().SingleInstance();
            builder.RegisterType<UpdateService>().As<IBackgroundWorker>().SingleInstance();

            // Services
            builder.RegisterType<QueryProviderService>().SingleInstance();
            builder.RegisterType<ApplicationService>().SingleInstance();
            builder.RegisterType<WindowService>().SingleInstance();
            builder.RegisterType<SettingsService>().AsSelf().AsImplementedInterfaces().SingleInstance();

            // Misc
            builder.RegisterType<JarvisTaskbarIcon>().SingleInstance();
            builder.RegisterType<TaskbarIconViewModel>().InstancePerDependency();

            // Seeders
            builder.RegisterType<GeneralSettingsSeeder>().As<ISettingsSeeder>().SingleInstance();

            // View models
            builder.RegisterAssemblyTypes(typeof(JarvisModule).Assembly)
                .Where(type => type.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
                .AsSelf().InstancePerDependency();

            // Register Views
            builder.RegisterAssemblyTypes(typeof(JarvisModule).Assembly)
                .Where(type => type.Name.EndsWith("View", StringComparison.Ordinal))
                .AsSelf().InstancePerDependency();
        }
    }
}
