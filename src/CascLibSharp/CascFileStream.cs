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
    /// <summary>
    /// Provides a view of the data in a file contained within CASC storage.
    /// </summary>
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

        /// <summary>
        /// Gets whether this Stream may be read.  Always returns true as long as the Stream has not been disposed.
        /// </summary>
        public override bool CanRead
        {
            get { return _handle != null && !_handle.IsInvalid; }
        }

        /// <summary>
        /// Gets whether this Stream may seek.  Always returns true as long as the Stream has not been disposed.
        /// </summary>
        public override bool CanSeek
        {
            get { return _handle != null && !_handle.IsInvalid; }
        }

        /// <summary>
        /// Gets whether this Stream may write.  Always returns false.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Flushes writes to the backing store.  This method always throws because writing is not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the length of the Stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the Stream has been disposed.</exception>
        public override long Length
        {
            get 
            {
                AssertValidHandle();
                return _api.CascGetFileSize(_handle); 
            }
        }

        /// <summary>
        /// Gets or sets the current position within the Stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the Stream has been disposed.</exception>
        /// <exception cref="CascException">Thrown if the attempt to seek fails.</exception>
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
                long result = _api.CascSetFilePointer(_handle, value, SeekOrigin.Begin);
                if (result != value)
                    throw new CascException();
            }
        }

        /// <summary>
        /// Reads data into the buffer.
        /// </summary>
        /// <param name="buffer">The destination into which the data should be read.</param>
        /// <param name="offset">The offset into the buffer to read from the current position of the Stream.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the Stream has been disposed.</exception>
        /// <exception cref="CascException">Thrown if the CASC provider fails to read.</exception>
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

        /// <summary>
        /// Seeks to a specific position within the Stream.
        /// </summary>
        /// <param name="offset">The offset from the specified origin to seek.</param>
        /// <param name="origin">The relative location (beginning, current, or end) from which to seek.</param>
        /// <returns>The new position in the stream.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the Stream has been disposed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            AssertValidHandle();
            return _api.CascSetFilePointer(_handle, offset, origin);
        }

        /// <summary>
        /// Sets the length of the Stream.  Always throws because writing is not supported.
        /// </summary>
        /// <param name="value">The new length of the Stream.</param>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes the specified data to the Stream.  Always throws because writing is not supported.
        /// </summary>
        /// <param name="buffer">The data to write.</param>
        /// <param name="offset">The starting position in the buffer.</param>
        /// <param name="count">The number of bytes.</param>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
