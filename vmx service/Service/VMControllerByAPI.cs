using System;
using System.Collections.Generic;
using System.Text;
using VMXService.Tools;
using VixCOM;

namespace VMXService.Service
{
    public class VMControllerByAPI : IVMController
    {
        const int IGNORED_POWER_FLAG =  
            Constants.VIX_POWERSTATE_TOOLS_RUNNING |
            Constants.VIX_POWERSTATE_RESETTING | // Always this flag is set, I don't know why.
            0x0400 | 0x030;

        IVixLib _api;
        IHost _host = null;

        #region map VMWareInfo types to VIX_SERVICEPROVIDER
        protected Dictionary<VMWareInfo.VMCoreTypes, int> map_type_to_VIX_SERVICEPROVIDER = new Dictionary<VMWareInfo.VMCoreTypes, int>()
        {
            {VMWareInfo.VMCoreTypes.UNKNOWN, Constants.VIX_SERVICEPROVIDER_DEFAULT},
            {VMWareInfo.VMCoreTypes.VMWarePlayer, Constants.VIX_SERVICEPROVIDER_VMWARE_PLAYER},
            {VMWareInfo.VMCoreTypes.VMWareWorkstation, Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION},
        };

        protected int GetVIXServiceProviderByType(VMWareInfo.VMCoreTypes type)
        {
            if (map_type_to_VIX_SERVICEPROVIDER.ContainsKey(type))
                return map_type_to_VIX_SERVICEPROVIDER[type];
            else
                return map_type_to_VIX_SERVICEPROVIDER[VMWareInfo.VMCoreTypes.UNKNOWN];
        }

        #endregion

        #region functions for VIX

        protected T GetResult<T>(Object[] vix_result)
        {
            if (vix_result == null) return default(T);
            return GetResult<T>(vix_result, 0);
        }

        protected T GetResult<T>(Object[] vix_result, uint n)
        {
            return (T)(vix_result[n]);
        }

        protected T GetVMProperty<T>(IVM vm, int property_id)
        {
            int[] properties = { property_id };
            Object result = null;
            
            ulong ret = ((IVixHandle)vm).GetProperties(properties, ref result);
            if (_api.ErrorIndicatesFailure(ret))
                return default(T);
                //throw new VMXServiceException("Unable to get vm status.");

            if (result == null) return default(T);
            else return (T)(((Object[])result)[0]);
        }

        protected Object[] WaitForResults(IJob job)
        {
            return WaitForResults(job, true);
        }

        protected Object[] WaitForResults(IJob job, bool free_job)
        {
            Object results = null;

            ulong ret = job.Wait(new int[] { Constants.VIX_PROPERTY_JOB_RESULT_HANDLE }, ref results);
            if (free_job) CloseVixObject(job);

            if (_api.ErrorIndicatesFailure(ret)) return null;

            return (Object[])results;
        }

        protected bool DoneJob(IJob job)
        {
            return DoneJob(job, true);
        }

        protected bool DoneJob(IJob job, bool free_job)
        {
            ulong ret = job.WaitWithoutResults();
            
            if (free_job) CloseVixObject(job);

            return _api.ErrorIndicatesSuccess(ret);
        }

        protected void CloseVixObject(Object vix_object)
        {
            try
            {
                ((IVixHandle2)vix_object).Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        public VMControllerByAPI(VMWareInfo.VMCoreTypes type)
        {
            try
            {
                _api = new VixLibClass();
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                throw new VMXServiceException(
                    "Unable to find Vix library!\n" + e.ToString());
            }

            int core = 1;// GetVIXServiceProviderByType(type);
            IJob job = _api.Connect(
                Constants.VIX_API_VERSION,
                core,
                null, 0, null, null, 0, null, null);

            _host = GetResult<VixCOM.IHost>(WaitForResults(job));
            if (_host == null)
                throw new VMXServiceException("Connecting to VMWare provider was failed.");

            CloseVixObject(job);
        }

        ~VMControllerByAPI()
        {
            CloseVixObject(_api);
        }

        protected bool IsRunning(string vmx)
        {
            IVM2 vm = GetResult<IVM2>(WaitForResults(_host.OpenVM(vmx, null)));
            bool is_running = GetVMProperty<bool>(vm, Constants.VIX_PROPERTY_VM_IS_RUNNING);
            CloseVixObject(vm);

            return is_running;
        }

        protected bool CanStop(string vmx)
        {
            if (!IsRunning(vmx)) return false;

            IVM vm = GetResult<IVM2>(WaitForResults(_host.OpenVM(vmx, null)));
            int power_state = GetVMProperty<int>(vm, Constants.VIX_PROPERTY_VM_POWER_STATE);

            int alt = power_state &
                ~(  Constants.VIX_POWERSTATE_POWERED_ON | 
                    IGNORED_POWER_FLAG);

            return alt == 0;
        }

        #region IVMController メンバ

        bool IVMController.IsRunning(string vmx)
        {
            return IsRunning(vmx);
        }

        bool IVMController.StartVMX(string vmx)
        {
            if (IsRunning(vmx)) return false;

            IVM2 vm = GetResult<IVM2>(WaitForResults(_host.OpenVM(vmx, null)));
            bool success = DoneJob(vm.PowerOn(Constants.VIX_VMPOWEROP_NORMAL, null, null));
            CloseVixObject(vm);

            return success && IsRunning(vmx);
        }

        bool IVMController.StopVMX(string vmx)
        {
            if (!CanStop(vmx)) return false;

            IVM2 vm = GetResult<IVM2>(WaitForResults(_host.OpenVM(vmx, null)));
            bool success = DoneJob(vm.PowerOff(Constants.VIX_VMPOWEROP_FROM_GUEST, null));
            CloseVixObject(vm);

            return success && !IsRunning(vmx);
        }

        bool IVMController.ContinueVMX(string vmx)
        {
            if (!IsRunning(vmx)) return false;

            IVM2 vm = GetResult<IVM2>(WaitForResults(_host.OpenVM(vmx, null)));
            bool success = DoneJob(vm.Unpause(0, null, null));
            CloseVixObject(vm);

            return success;
        }

        bool IVMController.PauseVMX(string vmx)
        {
            if (!IsRunning(vmx)) return false;

            IVM2 vm = GetResult<IVM2>(WaitForResults(_host.OpenVM(vmx, null)));
            bool success = DoneJob(vm.Pause(0, null, null));
            CloseVixObject(vm);

            return success;
        }

        #endregion
    }
}
