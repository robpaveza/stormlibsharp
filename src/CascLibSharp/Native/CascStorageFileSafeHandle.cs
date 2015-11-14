using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp.Native
{
    internal class CascStorageFileSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CascStorageFileSafeHandle()
            : base(true)
        {

        }

        public CascStorageFileSafeHandle(IntPtr handle)
            : this()
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            var api = Api ?? CascApi.Instance;
            return api.CascCloseFile(DangerousGetHandle());
        }

        internal CascApi Api
        {
            get;
            set;
        }
    }
}
