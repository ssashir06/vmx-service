using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMXService.Tools;
using VMXService.Service;
using VMXService.InstallTool;

namespace My_VMX_API_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string vmx = @"C:\UserData\data\disk-image\Virtual Machines\Ubuntu8.04LTS 64\Ubuntu8.04LTS 64.vmx";
            IVMController ctl = new VMControllerByAPI(VMXService.Tools.VMWareInfo.VMCoreTypes.VMWareWorkstation);

            Console.WriteLine("Is running: " + ctl.IsRunning(vmx));
        }
    }
}
