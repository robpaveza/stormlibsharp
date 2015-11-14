using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp.Native
{
    internal class CascStorageSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CascStorageSafeHandle()
            : base(true)
        {

        }

        public CascStorageSafeHandle(IntPtr handle)
            : this()
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            var api = Api ?? CascApi.Instance;
            return api.CascCloseStorage(DangerousGetHandle());
        }

        internal CascApi Api
        {
            get;
            set;
        }
    }
}
