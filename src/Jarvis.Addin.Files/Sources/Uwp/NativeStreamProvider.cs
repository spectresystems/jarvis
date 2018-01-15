// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Jarvis.Core.Interop;
using Spectre.System.IO;

namespace Jarvis.Addin.Files.Sources.Uwp
{
    internal sealed class NativeStreamProvider : INativeStreamProvider
    {
        public IStream GetStream(FilePath path)
        {
            const Win32.Shell.Stgm exclusiveRead = Win32.Shell.Stgm.Read | Win32.Shell.Stgm.ShareExclusive;
            var hResult = Win32.Shell.SHCreateStreamOnFileEx(path.FullPath, exclusiveRead, 0x80, false, null, out var stream);
            return hResult != Win32.Hresult.Ok ? null : stream;
        }

        public void ReleaseStream(IStream stream)
        {
            Marshal.ReleaseComObject(stream);
        }
    }
}