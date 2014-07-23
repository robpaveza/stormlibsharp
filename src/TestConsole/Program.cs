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
        static void Main(string[] args)
        {
            Console.WriteLine("Attach a native debugger now and press <enter> to continue.");
            Console.ReadLine();
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
}
