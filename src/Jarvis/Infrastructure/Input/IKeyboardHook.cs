// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Infrastructure.Input
{
    public interface IKeyboardHook : IDisposable
    {
        void Register();
    }
}
