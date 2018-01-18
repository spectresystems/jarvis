// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Jarvis.Core.Interop
{
    public static partial class Win32
    {
        public static class Shell
        {
            [Flags]
            public enum Stgm : uint
            {
                Read = 0x0,
                Write = 0x1,
                ReadWrite = 0x2,
                ShareExclusive = 0x10,
            }

            public enum FileAttribute
            {
                Directory = 0x00000010,
                Normal = 0x00000080
            }

            [Flags]
            public enum Shgfi
            {
                Icon = 0x000000100,
                DisplayName = 0x000000200,
                TypeName = 0x000000400,
                Attributes = 0x000000800,
                IconLocation = 0x000001000,
                ExeType = 0x000002000,
                SysIconIndex = 0x000004000,
                LinkOverlay = 0x000008000,
                Selected = 0x000010000,
                AttrSpecified = 0x000020000,
                LargeIcon = 0x000000000,
                SmallIcon = 0x000000001,
                OpenIcon = 0x000000002,
                ShellIconSize = 0x000000004,
                Pidl = 0x000000008,
                UseFileAttributes = 0x000000010,
                AddOverlays = 0x000000020,
                OverlayIndex = 0x000000040,
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
            [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
            public struct Shfileinfo
            {
#pragma warning disable SA1307
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
#pragma warning restore SA1307
            }

            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern Hresult SHCreateStreamOnFileEx(string pszFile, Stgm grfMode, uint dwAttributes,
                bool fCreate, IStream pstmTemplate, out IStream ppstm);

            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern Hresult SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf,
                uint cchOutBuf, IntPtr ppvReserved);

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern int SHGetFileInfo(
                string pszPath,
                FileAttribute dwFileAttributes,
                out Shfileinfo psfi,
                uint cbfileInfo,
                Shgfi uFlags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool DestroyIcon(IntPtr hIcon);
        }
    }
}