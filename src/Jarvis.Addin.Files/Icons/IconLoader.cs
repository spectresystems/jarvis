// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jarvis.Addin.Files.Icons
{
    internal sealed class IconLoader
    {
        private readonly MemoryCache _cache;

        public IconLoader()
        {
            _cache = new MemoryCache("FileIconCache");
        }

        public async Task<ImageSource> LoadAsync(FileResult file)
        {
            if (file.Icon == null)
            {
                return null;
            }

            if (_cache[file.Icon.AbsoluteUri] is ImageSource cachedImage)
            {
                return cachedImage;
            }

            var result = await LoadImageAsync(file).ConfigureAwait(false);
            if (result != null)
            {
                _cache.Set(file.Icon.AbsoluteUri, result, new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromSeconds(5),
#if DEBUG
                    RemovedCallback = arguments =>
                    {
                        Debug.WriteLine($"Evicted image {arguments.CacheItem.Key} from cache ({arguments.RemovedReason}).");
                    }
#endif
                });
            }

            return result;
        }

        private static async Task<ImageSource> LoadImageAsync(FileResult result)
        {
            if (result?.Icon == null)
            {
                return null;
            }

            var path = WebUtility.UrlDecode(result.Icon.AbsolutePath.TrimStart('/'));
            var parameters = result.Icon.GetQueryString();

            if (result.Icon.Scheme.Equals("shell", StringComparison.Ordinal))
            {
                var isDirectory = parameters.ContainsKey("isDirectory");
                return await ShellIconLoader.LoadImageAsync(path, isDirectory)
                    .ConfigureAwait(false);
            }

            if (result.Icon.Scheme.Equals("uwp", StringComparison.Ordinal))
            {
                parameters.TryGetValue("bg", out var bgColors);
                var bgColor = bgColors?.FirstOrDefault() ?? "Transparent";

                return UwpIconLoader.DrawIcon(path, bgColor);
            }

            return null;
        }
    }
}
