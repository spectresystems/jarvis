// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Spectre.System.IO;

// ReSharper disable once CheckNamespace
namespace Jarvis.Addin.Files
{
    internal static class FileSystemExtensions
    {
        public static IDirectory GetDirectorySafe(this IFileSystem fileSystem, DirectoryPath path)
        {
            try
            {
                return fileSystem.Directory.Get(path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<IFile> GetFilesSafe(this IFileSystem fileSystem, DirectoryPath root, string filter, SearchScope scope)
        {
            try
            {
                return fileSystem.Directory.GetFiles(root, filter, SearchScope.Current);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
