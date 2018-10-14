﻿// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Jarvis.Addin.Files;
using Jarvis.Addin.Google;
using Jarvis.Addin.Wikipedia;
using Jarvis.Bootstrapping;
using Jarvis.Core;
using Jarvis.Infrastructure.Utilities;
using Jarvis.Services;
using Jarvis.ViewModels;
using IContainer = Autofac.IContainer;

namespace Jarvis
{
    public class Bootstrapper : BootstrapperBase
    {
        private IContainer _container;
        private IDisposable _hotKey;
        private JarvisTaskbarIcon _taskbarIcon;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            // Initialize paths.
            PathUtility.Initialize();

            // Configure container.
            var builder = new ContainerBuilder();
            builder.RegisterInstance(Application).As<App>();
            builder.RegisterModule(new JarvisModule());
            builder.RegisterModule<UpdaterModule>();
            builder.RegisterModule<LoggingModule>();

            // Configure addins.
            builder.RegisterModule(new AddinModule(
                typeof(FileAddin).Assembly,
                typeof(GoogleAddin).Assembly,
                typeof(WikipediaAddin).Assembly));

            // Build the container.
            _container = builder.Build();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // Set custom namespace mappings.
            ViewLocator.AddNamespaceMapping("Jarvis.ViewModels.Settings", "Jarvis.Views.Settings");

            // Initialize everything that needs to.
            var initializables = IoC.GetAll<IInitializable>();
            foreach (var initializable in initializables)
            {
                initializable.Initialize();
            }

            // Show the root view.
            DisplayRootViewFor<ShellViewModel>();
            Application?.MainWindow?.Hide();

            // Create the taskbar icon.
            _taskbarIcon = IoC.Get<JarvisTaskbarIcon>();

            // Start all background services.
            IoC.Get<ServiceOrchestrator>().Start();

            // Register the hotkey.
            var service = IoC.Get<ApplicationService>();
            _hotKey = new KeyboardHook(() => service.Toggle());
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            // Unregister the hot key
            _hotKey?.Dispose();
            _taskbarIcon?.Dispose();

            // Stop the service orchestrator.
            var services = IoC.Get<ServiceOrchestrator>();
            services.Stop();
            services.Join();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                if (_container.IsRegistered(service))
                {
                    return _container.Resolve(service);
                }
            }
            else
            {
                if (_container.IsRegisteredWithName(key, service))
                {
                    return _container.ResolveNamed(key, service);
                }
            }
            throw new Exception($"Could not locate an instances of '{key ?? service.Name}'.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }
    }
}
