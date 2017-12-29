// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Extensions
{
    internal static class PathExtensions
    {
        public static Uri ToUri(this Path path, string scheme, IDictionary<string, string> parameters = null)
        {
            if (path != null)
            {
                var uri = $"{scheme}:///{WebUtility.UrlEncode(path.FullPath)}";
                if (parameters != null)
                {
                    var joinedParameters = parameters.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}");
                    uri = string.Concat(uri, "?", string.Join("&", joinedParameters));
                }
                return new Uri(uri);
            }
            return null;
        }
    }
}
