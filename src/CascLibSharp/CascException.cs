using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CascLibSharp
{
    /// <summary>
    /// Represents an exception raised as part of a CASC operation.
    /// </summary>
    public class CascException : Win32Exception
    {
        /// <summary>
        /// Creates a new CascException based on the last Win32 error.
        /// </summary>
        public CascException()
            : base(Marshal.GetLastWin32Error())
        {

        }

        /// <summary>
        /// Creates a new CascException based on the specified Win32 error code.
        /// </summary>
        /// <param name="errorCode">A Win32 error code.</param>
        public CascException(int errorCode)
            : base(errorCode)
        {

        }

        /// <summary>
        /// Creates a new CascException based on the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CascException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new CascException based on the specified Win32 error code and custom error message.
        /// </summary>
        /// <param name="errorCode">A Win32 error code.</param>
        /// <param name="message">The error message.</param>
        public CascException(int errorCode, string message)
            : base(errorCode, message)
        {

        }
    }
}
