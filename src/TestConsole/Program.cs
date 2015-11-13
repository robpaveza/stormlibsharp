using CascLibSharp;
using StormLibSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestConsole
{
    class Program
    {
        const string WOW_DATA_DIRECTORY_X64 = @"C:\Program Files (x86)\World of Warcraft\Data";
        const string HEROES_DATA_DIRECTORY_X64 = @"C:\Program Files (x86)\Heroes of the Storm\HeroesData";

        const string WOW_LISTFILE_PATH = @"C:\Projects\CascLib2\listfile\World of Warcraft 6x.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("Attach a native debugger now and press <enter> to continue.");
            Console.ReadLine();

            try
            {
                using (CascStorageContext casc = new CascStorageContext(WOW_DATA_DIRECTORY_X64))
                {
                    Console.WriteLine("Successfully loaded CASC storage context.");
                    Console.ReadLine();

                    using (var file = casc.OpenFile(@"Interface\GLUES\LOADINGSCREENS\LoadingScreen_HighMaulRaid.blp"))
                    {
                        File.WriteAllBytes("LoadingScreen_HighMaulRaid.blp", file.ReadAllBytes());
                    }
                    Console.WriteLine("Successfully extracted LoadingScreen_HighMaulRaid.blp");

                    foreach (var file in casc.SearchFiles("*", WOW_LISTFILE_PATH))
                    {
                        Console.WriteLine(file.FileName);
                    }
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            string listFile = null;
            using (MpqArchive archive = new MpqArchive(@"d:\Projects\base-Win.MPQ", FileAccess.Read))
            {
                using (MpqFileStream file = archive.OpenFile("(listfile)"))
                using (StreamReader sr = new StreamReader(file))
                {
                    listFile = sr.ReadToEnd();
                    Console.WriteLine(listFile);
                }

                archive.ExtractFile("(listfile)", @"d:\projects\base-win-listfile.txt");
            }

            using (MpqArchive archive = MpqArchive.CreateNew(@"d:\projects\mynewmpq.mpq", MpqArchiveVersion.Version4))
            {
                archive.AddFileFromDisk(@"D:\projects\base-win-listfile.txt", "base-win-listfile.txt");

                int retval = archive.AddListFile(@"base-win-listfile.txt");
                archive.Compact("base-win-listfile.txt");
                archive.Flush();
            }

            Console.ReadLine();
        }
    }

    internal static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream fs)
        {
            byte[] result = new byte[fs.Length];
            fs.Position = 0;
            int cur = 0;
            while (cur < fs.Length)
            {
                int read = fs.Read(result, cur, result.Length - cur);
                cur += read;
            }

            return result;
        }
    }
}
