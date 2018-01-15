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
using Jarvis.Core;
using Module = Autofac.Module;

namespace Jarvis.Bootstrapping
{
    public sealed class AddinModule : Module
    {
        private readonly Assembly[] _assemblies;

        public AddinModule(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies().Concat(_assemblies));
            foreach (var assembly in assemblies)
            {
                var modules = GetModulesInAssembly(assembly).ToArray();
                foreach (var module in modules)
                {
                    module.Configure(builder);
                }

                if (modules.Length > 0)
                {
                    // Make sure Caliburn.Micro can find external views.
                    if (!AssemblySource.Instance.Contains(assembly))
                    {
                        AssemblySource.Instance.Add(assembly);
                    }

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
