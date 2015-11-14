using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp.Native
{
    internal sealed class CoTaskMem : IDisposable
    {
        private IntPtr _mem;
        private int _size;

        public CoTaskMem(int size)
        {
            _mem = Marshal.AllocCoTaskMem(size);
            if (_mem == IntPtr.Zero)
                throw new OutOfMemoryException();

            _size = size;
        }

        public static CoTaskMem FromBytes(byte[] b)
        {
            CoTaskMem result = new CoTaskMem(b.Length);
            Marshal.Copy(b, 0, result._mem, b.Length);

            return result;
        }

        public IntPtr Pointer
        {
            get 
            {
                if (_mem == IntPtr.Zero)
                    throw new ObjectDisposedException("CoTaskMem");

                return _mem; 
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_mem != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(_mem);
                _mem = IntPtr.Zero;
                _size = 0;
            }
        }

        ~CoTaskMem()
        {
            Dispose(false);
        }
    }
}
