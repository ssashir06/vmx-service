using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace VMXService.Tools
{
    public class VMWareInfo
    {
        public enum VMCoreTypes
        {
            //VMWareServer,
            VMWarePlayer,
            VMWareWorkstation,
            UNKNOWN,
        };

        protected VMCoreTypes vmcore;

        protected Dictionary<string, VMCoreTypes> map_name_to_types = new Dictionary<string, VMCoreTypes>()
        {
            {"VMware Workstation", VMCoreTypes.VMWareWorkstation},
            {"VMware Player", VMCoreTypes.VMWarePlayer},
        };

        public VMWareInfo()
        {
            vmcore = GetVMCore();
        }

        public string PathToVMRun { get { return GetVMwareVIXDir() + "vmrun.exe"; } }
        public VMCoreTypes VMCore { get { return vmcore; } }

        protected VMCoreTypes GetTypeByName(string name)
        {
            if (map_name_to_types.ContainsKey(name))
                return map_name_to_types[name];
            else
                return VMCoreTypes.UNKNOWN;
        }

        protected string GetVMwareVIXDir()
        {
            RegistryKey reg = OpenVMWareRegistryKey("VMware VIX");
            string path = null;
            try
            {
                path = (string)reg.GetValue("InstallPath");
                if (!path.EndsWith(@"\")) path += @"\";
            }
            catch (VMXServiceException)
            {
                path = "";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reg.Close();
            }

            return path;
        }

        protected VMCoreTypes GetVMCore()
        {
            RegistryKey reg = OpenVMWareRegistryKey("");
            string core_name = null;
            try
            {
                core_name = (string)reg.GetValue("Core");
            }
            catch (VMXServiceException)
            {
                core_name = "";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reg.Close();
            }

            return GetTypeByName(core_name);
        }

        protected RegistryKey OpenVMWareRegistryKey(string sub_path)
        {
            Dictionary<string, string> path = new Dictionary<string, string>()
            {
                {"x86",   @"SOFTWARE\VMware, Inc.\"},
                {"AMD64", @"SOFTWARE\Wow6432Node\VMware, Inc.\"},
            };

            string arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            if (!path.ContainsKey(arch)) throw new VMXServiceException("Could not determine the cpu architecture.");

            return Registry.LocalMachine.OpenSubKey(path[arch] + sub_path);
        }
    }
}
