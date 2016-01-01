using System;
using System.Collections.Generic;
using System.Text;
using VMXService;

namespace VMXService.Service
{
    public interface IVMController : IDisposable
    {
        bool IsRunning(string vmx);
        bool StartVMX(string vmx);
        bool StopVMX(string vmx);
        bool ContinueVMX(string vmx);
        bool PauseVMX(string vmx);
    }
}
