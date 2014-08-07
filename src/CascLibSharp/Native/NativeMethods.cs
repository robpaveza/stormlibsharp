using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp.Native
{
    internal static class NativeMethods
    {
        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascOpenStorage(
            [MarshalAs(UnmanagedType.LPTStr)] string szDataPath,
            uint dwFlags,
            out CascStorageSafeHandle phStorage);

        // TODO: CascGetStorageInfo
        
        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascCloseStorage(IntPtr hStorage);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascOpenFileByIndexKey(
            CascStorageSafeHandle hStorage,
            ref QueryKey pIndexKey,
            uint dwFlags,
            out CascStorageFileSafeHandle phFile);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascOpenFileByEncodingKey(
            CascStorageSafeHandle hStorage,
            ref QueryKey pEncodingKey,
            uint dwFlags,
            out CascStorageFileSafeHandle phFile);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascOpenFile(
            CascStorageSafeHandle hStorage,
            [MarshalAs(UnmanagedType.LPStr)] string szFileName,
            uint dwLocale,
            uint dwFlags,
            out CascStorageFileSafeHandle phFile);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern uint CascGetFileSize(
            CascStorageFileSafeHandle hFile,
            out uint pdwFileSizeHigh);

        public static long CascGetFileSize(CascStorageFileSafeHandle hFile)
        {
            uint high;
            uint low = CascGetFileSize(hFile, out high);
            long result = (high << 32) | low;
            return result;
        }
        
        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern uint CascSetFilePointer(
            CascStorageFileSafeHandle hFile,
            uint lFilePos,
            ref uint plFilePosHigh,
            SeekOrigin dwMoveMethod);

        public static long CascSetFilePointer(
            CascStorageFileSafeHandle hFile,
            long filePos,
            SeekOrigin moveMethod)
        {
            uint low, high;
            unchecked
            {
                low = (uint)(filePos & 0xffffffff);
                high = (uint)(((ulong)filePos & 0xffffffff00000000) >> 32);
            }
            low = CascSetFilePointer(hFile, low, ref high, moveMethod);

            long result = (high << 32) | low;
            return result;
        }

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascReadFile(
            CascStorageFileSafeHandle hFile,
            IntPtr lpBuffer,
            uint dwToRead,
            out uint dwRead);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascCloseFile(IntPtr hFile);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern CascFileEnumerationSafeHandle CascFindFirstFile(
            CascStorageSafeHandle hStorage,
            [MarshalAs(UnmanagedType.LPStr)] string szMask,
            ref CascFindData pFindData,
            [MarshalAs(UnmanagedType.LPTStr)] string szListFile);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascFindNextFile(
            CascFileEnumerationSafeHandle hFind,
            ref CascFindData pFindData);

        [DllImport("CascLib", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
        public static extern bool CascFindClose(IntPtr hFind);
    }

    internal enum CascStorageInfoClass
    {
        FileCount,
        Features,
    }

#pragma warning disable 649
    internal struct QueryKey
    {
        public IntPtr pbData;
        public uint cbData;
    }

    internal unsafe struct CascFindData
    {
        const int MAX_PATH = 260;

        public fixed byte szFileName[MAX_PATH];
        public IntPtr szPlainName;
        public ulong FileNameHash;
        public fixed byte EncodingKey[16];
        public uint dwPackageIndex;
        public uint dwLocaleFlags;
        public uint dwFileSize;
    }
#pragma warning restore 649
}
