using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    public class CascFoundFile
    {
        internal CascFoundFile(string fileName, byte[] encodingKey, CascLocales locales, long fileSize)
        {
            FileName = fileName;
            EncodingKey = encodingKey;
            Locales = locales;
            FileSize = fileSize;
        }

        public string FileName { get; private set; }
        public byte[] EncodingKey { get; private set; }
        public CascLocales Locales { get; private set; }
        public long FileSize { get; private set; }
    }
}
