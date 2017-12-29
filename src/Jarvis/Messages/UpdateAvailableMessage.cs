// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Jarvis.Services.Updating;
using JetBrains.Annotations;

namespace Jarvis.Messages
{
    [UsedImplicitly]
    public sealed class UpdateAvailableMessage
    {
        public JarvisUpdateInfo UpdateInfo { get; }

        public UpdateAvailableMessage(JarvisUpdateInfo updateInfo)
        {
            UpdateInfo = updateInfo;
        }
    }
}
