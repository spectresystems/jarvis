// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Autofac;
using Jarvis.Core;

namespace Jarvis.Addin.Google
{
    public sealed class GoogleAddin : IAddin
    {
        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<GoogleProvider>().As<IQueryProvider>().SingleInstance();
        }
    }
}
