using CascLibSharp.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    internal sealed class CascApi
    {
        #region Imported method signature type definitions
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascOpenStorage(
                                                            [MarshalAs(UnmanagedType.LPTStr)] string szDataPath,
                                                            uint dwFlags,
                                                            out CascStorageSafeHandle phStorage);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascGetStorageInfo(
                                                            CascStorageSafeHandle hStorage,
                                                            CascStorageInfoClass infoClass,
                                                            ref uint pvStorageInfo,
                                                            IntPtr cbStorageInfo,
                                                            ref uint pcbLengthNeeded);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascCloseStorage(          IntPtr hStorage);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascOpenFileByIndexKey(
                                                            CascStorageSafeHandle hStorage,
                                                            ref QueryKey pIndexKey,
                                                            uint dwFlags,
                                                            out CascStorageFileSafeHandle phFile);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascOpenFileByEncodingKey(
                                                            CascStorageSafeHandle hStorage,
                                                            ref QueryKey pEncodingKey,
                                                            uint dwFlags,
                                                            out CascStorageFileSafeHandle phFile);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascOpenFile(
                                                            CascStorageSafeHandle hStorage,
                                                            [MarshalAs(UnmanagedType.LPStr)] string szFileName,
                                                            uint dwLocale,
                                                            uint dwFlags,
                                                            out CascStorageFileSafeHandle phFile);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint FnCascGetFileSize(
                                                            CascStorageFileSafeHandle hFile,
                                                            out uint pdwFileSizeHigh);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint FnCascSetFilePointer(
                                                            CascStorageFileSafeHandle hFile,
                                                            uint lFilePos,
                                                            ref uint plFilePosHigh,
                                                            SeekOrigin dwMoveMethod);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascReadFile(
                                                            CascStorageFileSafeHandle hFile,
                                                            IntPtr lpBuffer,
                                                            uint dwToRead,
                                                            out uint dwRead);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascCloseFile(             IntPtr hFile);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate CascFileEnumerationSafeHandle FnCascFindFirstFile(
                                                            CascStorageSafeHandle hStorage,
                                                            [MarshalAs(UnmanagedType.LPStr)] string szMask,
                                                            ref CascFindData pFindData,
                                                            [MarshalAs(UnmanagedType.LPTStr)] string szListFile);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascFindNextFile(
                                                            CascFileEnumerationSafeHandle hFind,
                                                            ref CascFindData pFindData);
        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool FnCascFindClose(             IntPtr hFind);
        #endregion

        #region Field and method defs
        public readonly FnCascOpenStorage CascOpenStorage;
        public readonly FnCascGetStorageInfo CascGetStorageInfo;
        public readonly FnCascCloseStorage CascCloseStorage;

        public readonly FnCascOpenFileByIndexKey CascOpenFileByIndexKey;
        public readonly FnCascOpenFileByEncodingKey CascOpenFileByEncodingKey;
        public readonly FnCascOpenFile CascOpenFile;

        public readonly FnCascGetFileSize CascGetFileSizeBase;
        public long CascGetFileSize(CascStorageFileSafeHandle hFile)
        {
            uint high;
            uint low = CascGetFileSizeBase(hFile, out high);
            long result = (high << 32) | low;

            return result;
        }
        public readonly FnCascSetFilePointer CascSetFilePointerBase;
        public long CascSetFilePointer(CascStorageFileSafeHandle hFile, long filePos, SeekOrigin moveMethod)
        {
            uint low, high;
            unchecked
            {
                low = (uint)(filePos & 0xffffffff);
                high = (uint)(((ulong)filePos & 0xffffffff00000000) >> 32);
            }

            low = CascSetFilePointerBase(hFile, low, ref high, moveMethod);

            long result = (high << 32) | low;

            return result;
        }
        public readonly FnCascReadFile CascReadFile;
        public readonly FnCascCloseFile CascCloseFile;

        public readonly FnCascFindFirstFile CascFindFirstFile;
        public readonly FnCascFindNextFile CascFindNextFile;
        public readonly FnCascFindClose CascFindClose;
        #endregion

        internal CascApi(IntPtr hModule)
        {
            SetFn(ref CascOpenStorage, hModule, "CascOpenStorage");
            SetFn(ref CascGetStorageInfo, hModule, "CascGetStorageInfo");
            SetFn(ref CascCloseStorage, hModule, "CascCloseStorage");

            SetFn(ref CascOpenFileByIndexKey, hModule, "CascOpenFileByIndexKey");
            SetFn(ref CascOpenFileByEncodingKey, hModule, "CascOpenFileByEncodingKey");
            SetFn(ref CascOpenFile, hModule, "CascOpenFile");
            SetFn(ref CascGetFileSizeBase, hModule, "CascGetFileSize");
            SetFn(ref CascSetFilePointerBase, hModule, "CascSetFilePointer");
            SetFn(ref CascReadFile, hModule, "CascReadFile");
            SetFn(ref CascCloseFile, hModule, "CascCloseFile");

            SetFn(ref CascFindFirstFile, hModule, "CascFindFirstFile");
            SetFn(ref CascFindNextFile, hModule, "CascFindNextFile");
            SetFn(ref CascFindClose, hModule, "CascFindClose");
        }

        private static void SetFn<T>(ref T target, IntPtr hModule, string procName)
            where T : class
        {
            IntPtr procAddr = NativeMethods.GetProcAddress(hModule, procName);
            if (procAddr == IntPtr.Zero)
                throw new Win32Exception();
            target = Marshal.GetDelegateForFunctionPointer(procAddr, typeof(T)) as T;
        }

        private static CascApi Load()
        {
            string myPath = Assembly.GetExecutingAssembly().Location;
            string directory = Path.GetDirectoryName(myPath);
            string arch = IntPtr.Size == 8 ? "x64" : "x86";
#if DEBUG
            string build = "dbg";
#else
            string build = "fre";
#endif

            string mainPath = Path.Combine(directory, "CascLib", build, arch, "CascLib.dll");
            if (File.Exists(mainPath))
                return FromFile(mainPath);

            string alternatePath = Path.Combine(directory, "CascLib", arch, "CascLib.dll");
            if (File.Exists(mainPath))
                return FromFile(alternatePath);

            string localPath = Path.Combine(directory, "CascLib.dll");
            if (File.Exists(localPath))
                return FromFile(localPath);

            throw new FileNotFoundException(string.Format("Could not locate a copy of CascLib.dll to load.  The following paths were tried:\n\t{0}\n\t{1}\n\t{2}\n\nEnsure that an architecture-appropriate copy of CascLib.dll is included in your project.", mainPath, alternatePath, localPath));
        }

        private static Lazy<CascApi> _sharedInstance = new Lazy<CascApi>(Load);
        public static CascApi Instance
        {
            get
            {
                return _sharedInstance.Value;
            }
        }

        public static CascApi FromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The CascLib.dll library could not be loaded at the specified path.", filePath);

            IntPtr hMod = NativeMethods.LoadLibraryEx(filePath, IntPtr.Zero, 0);
            if (hMod == IntPtr.Zero)
                throw new Win32Exception();

            return new CascApi(hMod);
        }
    }
}
