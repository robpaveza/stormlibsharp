using CascLibSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    public class CascStorageContext : IDisposable
    {
        private CascApi _api;
        private CascStorageSafeHandle _handle;
        private Lazy<bool> _hasListfile;
        private Lazy<CascKnownClient> _clientType;
        private Lazy<long> _fileCount;
        private Lazy<int> _gameBuild;

        public CascStorageContext(string dataPath)
        {
            _api = CascApi.Instance;

            if (!_api.CascOpenStorage(dataPath, 0, out _handle) || _handle.IsInvalid)
                throw new CascException();
            _handle.Api = _api;

            _hasListfile = new Lazy<bool>(CheckHasListfile);
            _clientType = new Lazy<CascKnownClient>(GetClient);
            _fileCount = new Lazy<long>(GetFileCount);
            _gameBuild = new Lazy<int>(GetGameBuild);
        }

        public CascFileStream OpenFile(string fileName, CascLocales locale = CascLocales.EnUs)
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            CascStorageFileSafeHandle hFile;
            if (!_api.CascOpenFile(_handle, fileName, (uint)locale, 0, out hFile))
                throw new CascException();

            hFile.Api = _api;

            return new CascFileStream(hFile, _api);
        }

        public CascFileStream OpenFileByIndexKey(byte[] indexKey)
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            using (CoTaskMem mem = CoTaskMem.FromBytes(indexKey))
            {
                QueryKey qk = new QueryKey();
                qk.cbData = unchecked((uint)indexKey.Length);
                qk.pbData = mem.Pointer;

                CascStorageFileSafeHandle hFile;
                if (!_api.CascOpenFileByIndexKey(_handle, ref qk, 0, out hFile))
                    throw new CascException();

                hFile.Api = _api;

                return new CascFileStream(hFile, _api);
            }
        }

        public CascFileStream OpenFileByEncodingKey(byte[] encodingKey)
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            using (CoTaskMem mem = CoTaskMem.FromBytes(encodingKey))
            {
                QueryKey qk = new QueryKey();
                qk.cbData = unchecked((uint)encodingKey.Length);
                qk.pbData = mem.Pointer;

                CascStorageFileSafeHandle hFile;
                if (!_api.CascOpenFileByEncodingKey(_handle, ref qk, 0, out hFile))
                    throw new CascException();

                hFile.Api = _api;

                return new CascFileStream(hFile, _api);
            }
        }

        public IEnumerable<CascFoundFile> SearchFiles(string mask, string listFilePath = null)
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            CascFindData cfd = new CascFindData();
            using (var handle = _api.CascFindFirstFile(_handle, mask, ref cfd, listFilePath))
            {
                if (handle.IsInvalid)
                    yield break;

                handle.Api = _api;

                yield return cfd.ToFoundFile();

                while (_api.CascFindNextFile(handle, ref cfd))
                {
                    yield return cfd.ToFoundFile();
                }
            }
        }
        
        private bool CheckHasListfile()
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            uint storageInfo = 0, lengthNeeded = 4;
            if (!_api.CascGetStorageInfo(_handle, CascStorageInfoClass.Features, ref storageInfo, new IntPtr(4), ref lengthNeeded))
                throw new CascException();

            CascStorageFeatures features = (CascStorageFeatures)storageInfo;
            if (features.HasFlag(CascStorageFeatures.HasListfile))
                return true;

            return false;
        }

        private CascKnownClient GetClient()
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            uint storageInfo = 0, lengthNeeded = 4;
            if (!_api.CascGetStorageInfo(_handle, CascStorageInfoClass.GameInfo, ref storageInfo, new IntPtr(4), ref lengthNeeded))
                throw new CascException();

            CascGameId gameId = (CascGameId)storageInfo;

            return gameId.ToKnownClient();
        }

        private long GetFileCount()
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            uint storageInfo = 0, lengthNeeded = 4;
            if (!_api.CascGetStorageInfo(_handle, CascStorageInfoClass.Features, ref storageInfo, new IntPtr(4), ref lengthNeeded))
                throw new CascException();

            return storageInfo;
        }

        private int GetGameBuild()
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            uint storageInfo = 0, lengthNeeded = 4;
            if (!_api.CascGetStorageInfo(_handle, CascStorageInfoClass.Features, ref storageInfo, new IntPtr(4), ref lengthNeeded))
                throw new CascException();

            return unchecked((int)storageInfo);
        }

        public bool HasListfile
        {
            get
            {
                return _hasListfile.Value;
            }
        }

        public long FileCount
        {
            get
            {
                return _fileCount.Value;
            }
        }

        public int GameBuild
        {
            get
            {
                return _gameBuild.Value;
            }
        }

        public CascKnownClient GameClient
        {
            get
            {
                return _clientType.Value;
            }
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

    public enum CascKnownClient
    {
        Unknown = -1,
        HeroesOfTheStorm = 0,
        Diablo3 = 1,
        WorldOfWarcraft = 2,
        Overwatch = 3,
    }
}
