using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Jarvis.Core.Interop;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace Jarvis.Tests.Utilities
{
    public class StreamWrapper : IStream
    {
        private readonly Stream _stream;

        public StreamWrapper(Stream stream)
        {
            _stream = stream;
        }

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            var read = _stream.Read(pv, 0, cb);
            if (pcbRead != IntPtr.Zero)
            {
                Marshal.WriteInt64(pcbRead, read);
            }
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            throw new NotImplementedException();
        }

        public void Seek(long dlibMove, int dwOrigin, IntPtr newPositionPtr)
        {
            var position = _stream.Seek(dwOrigin, (SeekOrigin)dwOrigin);
            if (newPositionPtr != IntPtr.Zero)
            {
                Marshal.WriteInt64(newPositionPtr, position);
            }
        }

        public void SetSize(long libNewSize)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotImplementedException();
        }

        public void Commit(int grfCommitFlags)
        {
            throw new NotImplementedException();
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new STATSTG
            {
                type = 2,
                cbSize = _stream.Length,
                grfMode = 0
            };

            if (_stream.CanRead)
            {
                pstatstg.grfMode |= (int)Win32.Stgm.Read;
            }
        }

        public void Clone(out IStream ppstm)
        {
            throw new NotImplementedException();
        }
    }
}
