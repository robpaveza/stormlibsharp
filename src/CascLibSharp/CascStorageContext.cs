using CascLibSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    public class CascStorageContext : IDisposable
    {
        private CascStorageSafeHandle _handle;

        public CascStorageContext(string dataPath)
        {
            if (!NativeMethods.CascOpenStorage(dataPath, 0, out _handle))
                throw new CascException();
        }

        public CascFileStream OpenFile(string fileName, CascLocales locale = CascLocales.EnUs)
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            CascStorageFileSafeHandle hFile;
            if (!NativeMethods.CascOpenFile(_handle, fileName, (uint)locale, 0, out hFile))
                throw new CascException();

            return new CascFileStream(hFile);
        }

        public IEnumerable<CascFoundFile> SearchFiles(string mask, string listFile)
        {
            yield break;
        }

        #region IDisposable implementation
        ~CascStorageContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_handle.IsInvalid)
                {
                    _handle.Close();
                    _handle = null;
                }
            }
        }
        #endregion
    }

    [Flags]
    public enum CascLocales
    {
        All = -1,
        None = 0,
        Unknown1 = 1,
        EnUs = 2,
        KoKr = 4,
        Unknown8 = 8,
        FrFr = 0x10,
        DeDe = 0x20,
        ZhCn = 0x40,
        EsEs = 0x80,
        ZhTw = 0x100,
        EnGb = 0x200,
        EnCn = 0x400,
        EnTw = 0x800,
        EsMx = 0x1000,
        RuRu = 0x2000,
        PtBr = 0x4000,
        ItIt = 0x8000,
        PtPt = 0x10000,
    }
}
