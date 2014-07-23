using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace StormLibSharp
{
    public class MpqArchive : IDisposable
    {
        #region Constructors / Factories
        public MpqArchive(string filePath, FileAccess accessType)
        {
            throw new NotImplementedException();
        }

        public MpqArchive(MemoryMappedFile file, FileAccess accessType)
        {
            throw new NotImplementedException();
        }

        public static MpqArchive CreateNew(string mpqPath, MpqArchiveVersion version)
        {
            throw new NotImplementedException();
        }

        public static MpqArchive CreateNew(string mpqPath, MpqArchiveVersion version, MpqFileStreamAttributes listfileAttributes,
            MpqFileStreamAttributes attributesFileAttributes, int maxFileCount)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        public int Locale
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int MaxFileCount
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsPatchedArchive
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void AddListFile(string listfileContents)
        {
            throw new NotImplementedException();
        }

        public void Compact(string listfile)
        {
            throw new NotImplementedException();
        }

        public event MpqArchiveCompactingEventHandler Compacting
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        // TODO: Determine if SFileGetAttributes/SFileSetAttributes/SFileUpdateFileAttributes deserves a projection.
        // It's unclear - these seem to affect the (attributes) file but I can't figure out exactly what that means.

        public void AddPatchArchive(string patchPath)
        {
            throw new NotImplementedException();
        }

        public void AddPatchArchives(IEnumerable<string> patchPaths)
        {
            throw new NotImplementedException();
        }

        public bool HasFile(string fileToFind)
        {
            throw new NotImplementedException();
        }

        public MpqFileStream OpenFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void ExtractFile(string fileToExtract, string destinationPath)
        {
            throw new NotImplementedException();
        }

        public bool VerifyFile(string fileToVerify)
        {
            throw new NotImplementedException();
        }

        // TODO: Consider SFileVerifyRawData

        public bool VerifyArchive()
        {
            throw new NotImplementedException();
        }




        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
        }

        ~MpqArchive()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release
            }
        }

        #endregion
    }

    public enum MpqArchiveVersion
    {
        Version1 = 0,
        Version2 = 0x01000000,
        Version3 = 0x02000000,
        Version4 = 0x03000000,
    }

    [Flags]
    public enum MpqFileStreamAttributes
    {
        None = 0x0,
    }

    
}
