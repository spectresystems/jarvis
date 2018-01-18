// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Jarvis.Addin.Files
{
    internal sealed class Constants
    {
        internal sealed class Settings
        {
            public const string Version = "Files.Version";

            internal sealed class Include
            {
                public const string Folders = "Files.Include.Folders";
                public const string Extensions = "Files.Include.Extensions";
            }

            internal sealed class Exclude
            {
                public const string Folders = "Files.Exclude.Folders";
                public const string Patterns = "Files.Exclude.Patterns";
            }
        }
    }
}
