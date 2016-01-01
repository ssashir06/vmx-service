using System;
using System.Diagnostics;

namespace VMXService
{
    class VMXServiceException : ApplicationException
    {
        public VMXServiceException(String message) : base(message) {
            try
            {
                Console.WriteLine(
                    "ERROR:\n" +
                    message);
                Trace.TraceError(message);
            }
            finally { }
        }
    }
}
