using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using VMXService.Tools;


// See http://stackoverflow.com/questions/773678/how-to-get-name-of-windows-service-from-inside-the-service-itself
// See http://stackoverflow.com/questions/652654/set-start-parameters-on-service-installation-with-net-serviceinstaller

namespace VMXService.InstallTool
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        ServiceInstaller _serviceInstaller = new ServiceInstaller();
        ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();

        public ProjectInstaller()
            : base()
        {
        }

        protected void ConfigureInstaller(string vmxname, string accountName)
        {
            _serviceInstaller.ServicesDependedOn = new string[] { "VMAuthdService" };
            _serviceInstaller.ServiceName = "VMXService(" + vmxname + ")";
            _serviceInstaller.DisplayName = "VMX Service (" + vmxname + ")";
            _serviceInstaller.DelayedAutoStart = true;
            _serviceInstaller.StartType = ServiceStartMode.Automatic;
            _serviceInstaller.Description = "A service launch specified VMX file as a automatic service.";

            _serviceProcessInstaller.Account = ServiceAccount.User;
            //_serviceProcessInstaller.Username = ".\\USERNAME_HERE!";
            _serviceProcessInstaller.Username = accountName;
            //_serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            Installers.AddRange(new Installer[] { _serviceProcessInstaller, _serviceInstaller });
        }

        protected void AddArguments(string parameters)
        {
            IntPtr hScm = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (hScm == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                IntPtr hSvc = OpenService(hScm, this._serviceInstaller.ServiceName, SERVICE_ALL_ACCESS);
                if (hSvc == IntPtr.Zero)
                    throw new Win32Exception();
                try
                {
                    QUERY_SERVICE_CONFIG oldConfig;
                    uint bytesAllocated = 8192;
                    IntPtr ptr = Marshal.AllocHGlobal((int)bytesAllocated);
                    try
                    {
                        uint bytesNeeded;
                        if (!QueryServiceConfig(hSvc, ptr, bytesAllocated, out bytesNeeded))
                        {
                            throw new Win32Exception();
                        }
                        oldConfig = (QUERY_SERVICE_CONFIG)Marshal.PtrToStructure(ptr, typeof(QUERY_SERVICE_CONFIG));
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }

                    string newBinaryPathAndParameters = oldConfig.lpBinaryPathName + " " + parameters;

                    if (!ChangeServiceConfig(hSvc, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE,
                        newBinaryPathAndParameters, null, IntPtr.Zero, null, null, null, null))
                        throw new Win32Exception();
                }
                finally
                {
                    if (!CloseServiceHandle(hSvc))
                        throw new Win32Exception();
                }
            }
            finally
            {
                if (!CloseServiceHandle(hScm))
                    throw new Win32Exception();
            }
        }

        protected bool IsValidName(string name)
        {
            return new Regex("^[a-zA-Z0-9_-]+$").IsMatch(name);
        }

        public override void Install(IDictionary stateSaver)
        {
            string parameters = null;
            string vmx_file = null, vm_target = null;
            string[] keys_needed = 
            {
                "VMXName", 
                "VMXFile", 
                //"Target", 
                "AccountName",
            };

            if (this.Context == null) throw new VMXServiceException("Installation infomation is not given.");

            vmx_file = this.Context.Parameters["VMXFile"];
            vm_target = this.Context.Parameters.ContainsKey("Target")
                ? this.Context.Parameters["Target"]
                : new VMTargetNames().GetShortNameByType(new VMWareInfo().VMCore);

            if (!IsValidName(this.Context.Parameters["VMXName"]))
                throw new VMXServiceException("Invalid name :'" + this.Context.Parameters["VMXName"] + "'.");
            if (new VMTargetNames().GetTypeByShortName(vm_target) == VMWareInfo.VMCoreTypes.UNKNOWN)
                throw new VMXServiceException("Invalid target name :'" + vm_target + "'.");

            foreach (string key in keys_needed)
            {
                if (!this.Context.Parameters.ContainsKey(key))
                {
                    throw new VMXServiceException(
                        "An essential key for installing is not set."+
                        "Please specify the key named '" + key + "',\n" +
                        "Like: installutil /" + key + "=VALUE");
                }
            }

            parameters = String.Join(
                " ",  
                Array.ConvertAll<string, string>(
                    new string[] { vmx_file, vm_target }, 
                    s => "\"" + s + "\""
                ));

            ConfigureInstaller(this.Context.Parameters["VMXName"], this.Context.Parameters["AccountName"]);
            stateSaver["VMXName"] = _serviceInstaller.ServiceName;

            base.Install(stateSaver);
            AddArguments(parameters);
        }

        public override void Rollback(IDictionary savedState)
        {
            ConfigureInstaller(this.Context.Parameters["VMXName"], this.Context.Parameters["AccountName"]);
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            ConfigureInstaller(this.Context.Parameters["VMXName"], this.Context.Parameters["AccountName"]);
            base.Uninstall(savedState);
        }

        #region setup Win32 Dlls
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenSCManager(
            string lpMachineName,
            string lpDatabaseName,
            uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenService(
            IntPtr hSCManager,
            string lpServiceName,
            uint dwDesiredAccess);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct QUERY_SERVICE_CONFIG
        {
            public uint dwServiceType;
            public uint dwStartType;
            public uint dwErrorControl;
            public string lpBinaryPathName;
            public string lpLoadOrderGroup;
            public uint dwTagId;
            public string lpDependencies;
            public string lpServiceStartName;
            public string lpDisplayName;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryServiceConfig(
            IntPtr hService,
            IntPtr lpServiceConfig,
            uint cbBufSize,
            out uint pcbBytesNeeded);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeServiceConfig(
            IntPtr hService,
            uint dwServiceType,
            uint dwStartType,
            uint dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseServiceHandle(
            IntPtr hSCObject);

        private const uint SERVICE_NO_CHANGE = 0xffffffffu;
        private const uint SC_MANAGER_ALL_ACCESS = 0xf003fu;
        private const uint SERVICE_ALL_ACCESS = 0xf01ffu;
        #endregion
    }
}
