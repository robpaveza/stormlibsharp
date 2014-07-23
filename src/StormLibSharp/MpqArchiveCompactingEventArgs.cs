﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StormLibSharp
{
    public delegate void MpqArchiveCompactingEventHandler(MpqArchive sender, MpqArchiveCompactingEventArgs e);

    public class MpqArchiveCompactingEventArgs : EventArgs
    {
        public MpqCompactingWorkType WorkType
        {
            get;
            private set;
        }

        public long BytesProcessed
        {
            get;
            private set;
        }

        public long TotalBytes
        {
            get;
            private set;
        }
    }

    public enum MpqCompactingWorkType
    {
        CheckingFiles = 1,
        CheckingHashTable = 2,
        CopyingNonMpqData = 3,
        CompactingFiles = 4,
        ClosingArchive = 5,
    }
}