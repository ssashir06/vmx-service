using System;

namespace VMXService.Installer
{
    class InstallerException : ApplicationException
    {
        public InstallerException(String message) : base(message) { }
    }
}
