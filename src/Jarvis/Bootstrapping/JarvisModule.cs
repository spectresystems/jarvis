// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using Caliburn.Micro;
using Jarvis.Bootstrapping.Seeding;
using Jarvis.Core;
using Jarvis.Core.Threading;
using Jarvis.Services;
using Jarvis.Services.Updating;
using Jarvis.ViewModels;
using Spectre.System.IO;
using Module = Autofac.Module;

namespace Jarvis.Bootstrapping
{
    public sealed class JarvisModule : Module
    {
        private readonly Assembly[] _assemblies;

        public JarvisModule(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

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

#if !DEBUG || FAKERELEASE
            builder.RegisterType<UpdateChecker>().As<IUpdateChecker>().SingleInstance();
#else
#if HAS_UPDATE
            builder.RegisterType<FakeUpdateChecker.WithUpdate>().As<IUpdateChecker>().SingleInstance();
#else
            builder.RegisterType<FakeUpdateChecker.WithoutUpdate>().As<IUpdateChecker>().SingleInstance();
#endif
#endif

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

            // Register view models and views.
            foreach (var assembly in _assemblies.Concat(new[] { typeof(JarvisModule).Assembly }))
            {
                // View models
                builder.RegisterAssemblyTypes(assembly)
                    .Where(type => type.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                    .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
                    .AsSelf().InstancePerDependency();

                // Register Views
                builder.RegisterAssemblyTypes(assembly)
                    .Where(type => type.Name.EndsWith("View", StringComparison.Ordinal))
                    .AsSelf().InstancePerDependency();
            }

            // Load addins.
            LoadAddins(builder);
        }

        private void LoadAddins(ContainerBuilder builder)
        {
            var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies().Concat(_assemblies));
            foreach (var assembly in assemblies)
            {
                foreach (var module in GetModulesInAssembly(assembly))
                {
                    module.Configure(builder);
                }

                // Make sure Caliburn.Micro can find external views.
                if (!AssemblySource.Instance.Contains(assembly))
                {
                    AssemblySource.Instance.Add(assembly);
                }
            }
        }

        private static IEnumerable<IAddin> GetModulesInAssembly(Assembly assembly)
        {
            foreach (var attribute in assembly.GetCustomAttributes<AddinAttribute>())
            {
                if (!typeof(IAddin).IsAssignableFrom(attribute.ModuleType))
                {
                    continue;
                }

                var defaultConstructor = attribute.ModuleType.GetConstructor(Type.EmptyTypes);
                if (defaultConstructor == null)
                {
                    throw new InvalidOperationException("Could not find default constructor.");
                }

                yield return Activator.CreateInstance(attribute.ModuleType) as IAddin;
            }
        }
    }
}
