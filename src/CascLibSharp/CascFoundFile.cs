using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    /// <summary>
    /// Represents a file found in a CASC container search.
    /// </summary>
    public class CascFoundFile
    {
        private WeakReference<CascStorageContext> _ownerContext;

        internal CascFoundFile(string fileName, IntPtr plainName, byte[] encodingKey, CascLocales locales, long fileSize, CascStorageContext ownerContext)
        {
            FileName = fileName;
            PlainFileName = Marshal.PtrToStringAnsi(plainName);
            EncodingKey = encodingKey;
            Locales = locales;
            FileSize = fileSize;

            _ownerContext = new WeakReference<CascStorageContext>(ownerContext);
        }

        /// <summary>
        /// Gets the full path to this file.
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// Gets the plain (no directory-qualified) file name of this file.
        /// </summary>
        public string PlainFileName { get; private set; }
        /// <summary>
        /// Gets the CASC encoding key for this file.
        /// </summary>
        public byte[] EncodingKey { get; private set; }
        /// <summary>
        /// Gets the locales supported by this resource.
        /// </summary>
        public CascLocales Locales { get; private set; }
        /// <summary>
        /// Gets the length of the file in bytes.
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// Opens the found file for reading.
        /// </summary>
        /// <returns>A CascFileStream, which acts as a Stream for a CASC stored file.</returns>
        public CascFileStream Open()
        {
            CascStorageContext context;
            if (!_ownerContext.TryGetTarget(out context))
                throw new ObjectDisposedException("The owning context has been closed.");

            return context.OpenFileByEncodingKey(EncodingKey);
        }
    }
}
