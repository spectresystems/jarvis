using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jarvis.Core.Interop
{
    public static partial class Win32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int access, bool inherit, int proccessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr QueryFullProcessImageName(IntPtr hprocess, int dwFlags, [Out] StringBuilder lpExeName, [Out] [MarshalAs(UnmanagedType.U4)] out int size);
    }
}