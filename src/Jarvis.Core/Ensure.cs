// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;

namespace Jarvis.Core
{
    public static class Ensure
    {
        public static void NotNull<T>(T item, [NotNull]string name)
            where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
