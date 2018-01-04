// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace Jarvis.Core
{
    public static class SettingStoreExtensions
    {
        public static T Get<T>(this ISettingsStore service, string name, T defaultValue = default)
        {
            var value = service.Get(name);
            if (value != null)
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromInvariantString(value);
            }
            return defaultValue;
        }

        public static void Set<T>(this ISettingsStore service, string name, T value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            service.Set(name, converter.ConvertToInvariantString(value));
        }
    }
}
