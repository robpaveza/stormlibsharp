using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StormLibSharp
{
    public class MpqFileStream : Stream
    {
        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        // TODO: Seems like the right place for SFileGetFileInfo, but will need to determine
        // what value add these features have except for sophisticated debugging purposes 
        // (like in Ladis' MPQ Editor app).

        public int ChecksumCrc32
        {
            get
            {
                throw new NotImplementedException(); 
            }
        }

        public byte[] GetMd5Hash()
        {
            throw new NotImplementedException();
        }
    }
}
