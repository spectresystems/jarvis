// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AddinAttribute : Attribute
    {
        public Type ModuleType { get; }

        public AddinAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }
    }
}