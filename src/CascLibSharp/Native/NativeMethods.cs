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
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
    }

    internal enum CascStorageInfoClass
    {
        FileCount,
        Features,
        GameInfo,
        GameBuild,
    }

    internal enum CascStorageFeatures
    {
        None = 0x0,
        HasListfile = 0x1,
    }

    /*
     * #define CASC_GAME_HOTS      0x00010000          // Heroes of the Storm
#define CASC_GAME_WOW6      0x00020000          // World of Warcraft - Warlords of Draenor
#define CASC_GAME_DIABLO3   0x00030000          // Diablo 3 since PTR 2.2.0
#define CASC_GAME_OVERWATCH 0x00040000          // Overwatch since PTR 24919*
     */
    internal enum CascGameId
    {
        Hots = 0x00010000,
        Wow6 = 0x00020000,
        Diablo3 = 0x00030000,
        Overwatch = 0x00040000,
        Starcraft2 = 0x00050000,
    }

    internal static class GameConverterExtensions
    {
        private static Dictionary<CascGameId, CascKnownClient> GameClientMap = new Dictionary<CascGameId, CascKnownClient> 
        {
            { CascGameId.Hots, CascKnownClient.HeroesOfTheStorm },
            { CascGameId.Wow6, CascKnownClient.WorldOfWarcraft },
            { CascGameId.Diablo3, CascKnownClient.Diablo3 },
            { CascGameId.Overwatch, CascKnownClient.Overwatch },
            { CascGameId.Starcraft2, CascKnownClient.Starcraft2 },
        };

        private static Dictionary<CascKnownClient, CascGameId> ClientGameMap = new Dictionary<CascKnownClient, CascGameId>() 
        {
            { CascKnownClient.HeroesOfTheStorm, CascGameId.Hots },
            { CascKnownClient.WorldOfWarcraft, CascGameId.Wow6 },
            { CascKnownClient.Diablo3, CascGameId.Diablo3 },
            { CascKnownClient.Overwatch, CascGameId.Overwatch },
            { CascKnownClient.Starcraft2, CascGameId.Starcraft2 },
        };

        public static CascKnownClient ToKnownClient(this CascGameId gameId)
        {
            CascKnownClient result;
            if (!GameClientMap.TryGetValue(gameId, out result))
                result = CascKnownClient.Unknown;

            return result;
        }

        public static CascGameId ToGameId(this CascKnownClient knownClient)
        {
            CascGameId result;
            if (!ClientGameMap.TryGetValue(knownClient, out result))
                throw new ArgumentException("Invalid client.");

            return result;
        }
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
        //public ulong FileNameHash;
        public fixed byte EncodingKey[16];
        //public uint dwPackageIndex;
        public uint dwLocaleFlags;
        public uint dwFileSize;

        public unsafe CascFoundFile ToFoundFile(CascStorageContext ownerContext)
        {
            string fileName = null;
            fixed (void* pFileName = szFileName)
            {
                fileName = Marshal.PtrToStringAnsi(new IntPtr(pFileName));
            }
            byte[] encodingKey = new byte[16];
            fixed (void* pEncodingKey = EncodingKey)
            {
                Marshal.Copy(new IntPtr(pEncodingKey), encodingKey, 0, 16);
            }

            return new CascFoundFile(fileName, szPlainName, encodingKey, (CascLocales)dwLocaleFlags, dwFileSize, ownerContext);
        }
    }
#pragma warning restore 649
}
