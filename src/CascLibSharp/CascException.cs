using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    public class CascException : Win32Exception
    {
        public CascException()
            : base()
        {

        }

        public CascException(int errorCode)
            : base(errorCode)
        {

        }

        public CascException(string message)
            : base(message)
        {

        }

        public CascException(int errorCode, string message)
            : base(errorCode, message)
        {

        }
    }
}
