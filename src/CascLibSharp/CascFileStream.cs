using CascLibSharp.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    public class CascFileStream : Stream
    {
        private CascStorageFileSafeHandle _handle;
        private CascApi _api;

        internal CascFileStream(CascStorageFileSafeHandle handle, CascApi api)
        {
            Debug.Assert(handle != null);
            Debug.Assert(!handle.IsInvalid);
            Debug.Assert(api != null);

            _api = api;
            _handle = handle;
        }

        private void AssertValidHandle()
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascFileStream");
        }

        public override bool CanRead
        {
            get { return _handle != null && !_handle.IsInvalid; }
        }

        public override bool CanSeek
        {
            get { return _handle != null && !_handle.IsInvalid; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get 
            {
                AssertValidHandle();
                return _api.CascGetFileSize(_handle); 
            }
        }

        public override long Position
        {
            get
            {
                AssertValidHandle();
                return _api.CascSetFilePointer(_handle, 0, SeekOrigin.Current);
            }
            set
            {
                AssertValidHandle();
                _api.CascSetFilePointer(_handle, value, SeekOrigin.Begin);
            }
        }

        public override unsafe int Read(byte[] buffer, int offset, int count)
        {
            AssertValidHandle();
            long len = Length;
            if (offset < 0 || offset > len)
                throw new ArgumentException("offset");
            if (count < 0)
                throw new ArgumentException("count");
            if (offset + count > len)
                throw new ArgumentException("count");

            uint read = 0;
            fixed (byte* pBuffer = &buffer[0])
            {
                if (!_api.CascReadFile(_handle, new IntPtr((void*)pBuffer), unchecked((uint)count), out read))
                    throw new CascException();
            }

            return unchecked((int)read);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            AssertValidHandle();
            return _api.CascSetFilePointer(_handle, offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
