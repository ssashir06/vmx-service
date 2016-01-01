using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VMXService.Tools;

namespace VMXService.Service
{
    public class VMControllerByVMRun : IVMController
    {
        protected string _vmrun_path = null;
        protected string _vmware_core = null;

        #region map core type to vmrun's target
        protected Dictionary<VMWareInfo.VMCoreTypes, string> map_type_to_target = new Dictionary<VMWareInfo.VMCoreTypes, string>()
        {
            {VMWareInfo.VMCoreTypes.UNKNOWN, "ws"},
            {VMWareInfo.VMCoreTypes.VMWarePlayer, "player"},
            {VMWareInfo.VMCoreTypes.VMWareWorkstation, "ws"},
        };

        protected string GetTargetNameByType(VMWareInfo.VMCoreTypes type)
        {
            if (map_type_to_target.ContainsKey(type))
                return map_type_to_target[type];
            else
                return map_type_to_target[VMWareInfo.VMCoreTypes.UNKNOWN];
        }
        #endregion

        public VMControllerByVMRun(VMWareInfo.VMCoreTypes type)
        {
            VMWareInfo vm_info = new VMWareInfo();
            _vmware_core = GetTargetNameByType(type);
            _vmrun_path = vm_info.PathToVMRun;
        }

        protected bool IsRunning(string vmx)
        {
            if (!File.Exists(vmx)) return false;
            string[] arg = { "list" };
            return Run(arg).IndexOf(vmx) >= 0;
        }

        protected string Run(params string[] args)
        {
            using (var proc = new Process())
            {
                proc.StartInfo.FileName = _vmrun_path;
                proc.StartInfo.Arguments = String.Join(" ", args);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                try
                {
                    proc.Start();
                    var reading = proc.StandardOutput.ReadToEndAsync();
                    proc.WaitForExit();
                    return reading.Result;
                }
                catch
                {
                    throw;
                }
            }
        }

        #region IVMController メンバ

        bool IVMController.IsRunning(string vmx)
        {
            return IsRunning(vmx);
        }

        bool IVMController.StartVMX(string vmx)
        {
            if (!File.Exists(vmx)) return false;

            string[] arg = {
                               "-T", _vmware_core,
                               "start", '"' + vmx + '"',
                               "nogui"
                           };

            Run(arg);
            return IsRunning(vmx);
        }

        bool IVMController.StopVMX(string vmx)
        {
            if (!File.Exists(vmx)) return false;

            string[] arg = {
                               "-T", _vmware_core,
                               "stop", '"' + vmx + '"',
                               "nogui"
                           };

            Run(arg);
            return !IsRunning(vmx);
        }

        bool IVMController.ContinueVMX(string vmx)
        {
            if (!File.Exists(vmx)) return false;
            if (!IsRunning(vmx)) return false;

            string[] arg = {
                               "-T", _vmware_core,
                               "unpause", '"' + vmx + '"',
                               "nogui"
                           };
            Run(arg);

            return true;
        }

        bool IVMController.PauseVMX(string vmx)
        {
            if (!File.Exists(vmx)) return false;
            if (!IsRunning(vmx)) return false;

            string[] arg = {
                               "-T", _vmware_core,
                               "pause", '"' + vmx + '"',
                               "nogui"
                           };

            Run(arg);

            return true;
        }

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
