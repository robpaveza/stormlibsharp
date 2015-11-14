using CascLibSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    /// <summary>
    /// Represents a CASC storage directory.
    /// </summary>
    public class CascStorageContext : IDisposable
    {
        private CascApi _api;
        private CascStorageSafeHandle _handle;
        private Lazy<bool> _hasListfile;
        private Lazy<CascKnownClient> _clientType;
        private Lazy<long> _fileCount;
        private Lazy<int> _gameBuild;

        /// <summary>
        /// Creates a new CascStorageContext for the specified path.
        /// </summary>
        /// <param name="dataPath">The path to a game's data directory.</param>
        /// <example>An example directory is <c>c:\Program Files (x86)\Heroes of the Storm\HeroesData</c>.</example>
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

        /// <summary>
        /// Opens a file by its fully-qualified name.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="locale">The file's locale (defaults to English-United States).</param>
        /// <returns>A CascFileStream, which implements a Stream.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the CascStorageContext has been disposed.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file does not exist within the CASC storage container.</exception>
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

        /// <summary>
        /// Opens a file by its index key.
        /// </summary>
        /// <remarks>
        /// An index key is a binary representation of a file.  I do not know what it comes from; I know that it's used to identify files inside of CASC, 
        /// but I don't know how someone obtains the index key.  They are not produced in the public API of CascLib.
        /// </remarks>
        /// <param name="indexKey">The index key to search.</param>
        /// <returns>A CascFileStream, which implements a Stream.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the CascStorageContext has been disposed.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file does not exist within the CASC storage container.</exception>
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

        /// <summary>
        /// Opens a file by its encoding key.
        /// </summary>
        /// <param name="encodingKey">A 16-byte key representing the file.  Encoding keys may be obtained via <see cref="SearchFiles(string, string)"/>.</param>
        /// <returns>A CascFileStream, which implements a Stream.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the CascStorageContext has been disposed.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file does not exist within the CASC storage container.</exception>
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

        /// <summary>
        /// Searches the files in the CASC container for files that match the specified pattern.
        /// </summary>
        /// <param name="mask">The mask to search.  * and ? are valid tokens for substitution.</param>
        /// <param name="listFilePath">A path to a listfile.  Required if the CASC container is for World of Warcraft.</param>
        /// <returns>An enumeration of matching file references in the CASC container.</returns>
        public IEnumerable<CascFoundFile> SearchFiles(string mask, string listFilePath = null)
        {
            if (_handle == null || _handle.IsInvalid)
                throw new ObjectDisposedException("CascStorageContext");

            if (this.GameClient == CascKnownClient.WorldOfWarcraft && string.IsNullOrWhiteSpace(listFilePath))
                throw new ArgumentNullException("listFilePath");

            CascFindData cfd = new CascFindData();
            using (var handle = _api.CascFindFirstFile(_handle, mask, ref cfd, listFilePath))
            {
                if (handle.IsInvalid)
                    yield break;

                handle.Api = _api;

                yield return cfd.ToFoundFile(this);

                while (_api.CascFindNextFile(handle, ref cfd))
                {
                    yield return cfd.ToFoundFile(this);
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

        /// <summary>
        /// Gets whether the CASC container has a listfile or if one must be supplied while searching.
        /// </summary>
        public bool HasListfile
        {
            get
            {
                return _hasListfile.Value;
            }
        }

        /// <summary>
        /// Gets the number of files in the CASC container.
        /// </summary>
        public long FileCount
        {
            get
            {
                return _fileCount.Value;
            }
        }

        /// <summary>
        /// Gets the build number of the game.
        /// </summary>
        public int GameBuild
        {
            get
            {
                return _gameBuild.Value;
            }
        }

        /// <summary>
        /// Gets the game client of the container, if it can be determined.
        /// </summary>
        public CascKnownClient GameClient
        {
            get
            {
                return _clientType.Value;
            }
        }

        #region IDisposable implementation
        /// <summary>
        /// Finalizes the storage context.
        /// </summary>
        ~CascStorageContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object, cleaning up the unmanaged objects.
        /// </summary>
        /// <param name="disposing">True if this is being called via the Dispose() method; false if it's being called by the finalizer.</param>
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

    /// <summary>
    /// Known locales supported by CASC.
    /// </summary>
    [Flags]
    public enum CascLocales
    {
        /// <summary>
        /// All available locales.
        /// </summary>
        All = -1,
        /// <summary>
        /// No locales.
        /// </summary>
        None = 0,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown1 = 1,
        /// <summary>
        /// English, United States
        /// </summary>
        EnUs = 2,
        /// <summary>
        /// Korean, South Korea
        /// </summary>
        KoKr = 4,
        /// <summary>
        /// Reserved (unknown)
        /// </summary>
        Reserved = 8,
        /// <summary>
        /// French, France
        /// </summary>
        FrFr = 0x10,
        /// <summary>
        /// German, Germany
        /// </summary>
        DeDe = 0x20,
        /// <summary>
        /// Chinese, China
        /// </summary>
        ZhCn = 0x40,
        /// <summary>
        /// Spanish, Spain
        /// </summary>
        EsEs = 0x80,
        /// <summary>
        /// Chinese, Taiwan
        /// </summary>
        ZhTw = 0x100,
        /// <summary>
        /// English, Great Britain
        /// </summary>
        EnGb = 0x200,
        /// <summary>
        /// English, China
        /// </summary>
        EnCn = 0x400,
        /// <summary>
        /// English, Taiwan
        /// </summary>
        EnTw = 0x800,
        /// <summary>
        /// Spanish, Mexico
        /// </summary>
        EsMx = 0x1000,
        /// <summary>
        /// Russian, Russia
        /// </summary>
        RuRu = 0x2000,
        /// <summary>
        /// Portuguese, Brazil
        /// </summary>
        PtBr = 0x4000,
        /// <summary>
        /// Italian, Italy
        /// </summary>
        ItIt = 0x8000,
        /// <summary>
        /// Portuguese, Portugal
        /// </summary>
        PtPt = 0x10000,
    }

    /// <summary>
    /// Known clients supporting CASC
    /// </summary>
    public enum CascKnownClient
    {
        /// <summary>
        /// The game client was unrecognized.
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Heroes of the Storm
        /// </summary>
        HeroesOfTheStorm = 0,
        /// <summary>
        /// Diablo 3
        /// </summary>
        Diablo3 = 1,
        /// <summary>
        /// World of Warcraft
        /// </summary>
        WorldOfWarcraft = 2,
        /// <summary>
        /// Overwatch
        /// </summary>
        Overwatch = 3,
        /// <summary>
        /// Starcraft 2
        /// </summary>
        Starcraft2 = 4,
    }
}
