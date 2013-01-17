using System;

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
            }
            finally
            {
            }
        }
    }
}
