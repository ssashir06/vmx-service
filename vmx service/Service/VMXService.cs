using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using VMXService.Tools;

namespace VMXService.Service
{
    public partial class Service : ServiceBase
    {
        protected string _vmx_file = null;
        protected IVMController _control = null;

        protected bool _invalid_state = false;
        protected Timer _timer = null;

        public Service()
            : base()
        {
            SetServiceBehavior();
            _invalid_state = true;

            WriteLog(
                "Invalid arguments!\n" +
                String.Join("\n", Environment.GetCommandLineArgs()), EventLogEntryType.FailureAudit);
        }

        public Service(string vmx_file, VMWareInfo.VMCoreTypes vm_target)
            : base()
        {
            SetServiceBehavior();
            _vmx_file = vmx_file;

            try
            {
                //_control = new VMControllerByVMRun(vm_target);
                _control = new VMControllerByAPI(vm_target);
            }
            catch (VMXServiceException)
            {
                _invalid_state = true;
                WriteLog("Failed To Initialize VMWare controller!", EventLogEntryType.Error);
                return;
            }

            if (!File.Exists(_vmx_file))
            {
                WriteLog("Specified vmx file '" + _vmx_file + "' does not exists!", EventLogEntryType.FailureAudit);
                _invalid_state = true;
                return;
            }

            _timer = new Timer(20000);// 20 sec.
            _timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
            _timer.Enabled = false;
        }

        protected void SetServiceBehavior()
        {
            AutoLog = false;
            CanPauseAndContinue = true;
            ServiceName = "vmx service";
        }

        protected void OnTimerEvent(object source, ElapsedEventArgs e)
        {
            if (!_control.IsRunning(_vmx_file))
            {
                _invalid_state = true;
                WriteLog(
                    "The VMX should be running, but not.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Warning);
                Stop();
            }
        }

        protected override void OnStart(string[] args)
        {
            if (_invalid_state)
            {
                Stop();
                return;
            }

            if (_control.IsRunning(_vmx_file))
            {
                WriteLog(
                    "The VMX file is aleady running.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Information);
            }
            else if (!_control.StartVMX(_vmx_file))
            {
                WriteLog(
                    "Starting specified vmx was failed.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Error);
                Stop();
            }
            else
            {
                WriteLog(
                    "Now starting specified vmx.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Information);
                _timer.Enabled = true;
            }
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;

            if (_invalid_state) return;

            if (!_control.IsRunning(_vmx_file))
            {
                WriteLog(
                    "Specified vmx might not be running.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.FailureAudit);
                return;
            }

            if (!_control.StopVMX(_vmx_file))
            {
                WriteLog(
                    "Stopping the vmx was failed.\n" + 
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Error);

            }
            else 
            {
                WriteLog(
                    "Stopping specified vmx was finished successfully.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Information);
            }
        }

        protected override void OnPause()
        {
            if (_invalid_state) return;

            if (!_control.PauseVMX(_vmx_file))
            {
                WriteLog(
                    "Pausing specified vmx was failed.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Error);

            }
            else
            {
                WriteLog(
                    "Pausing specified vmx was finished successfully.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Information);
            }
        }

        protected override void OnContinue()
        {
            if (_invalid_state) return;

            if (!_control.ContinueVMX(_vmx_file))
            {
                WriteLog(
                    "Continuing specified vmx was failed.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Error);

            }
            else
            {
                WriteLog(
                    "Continuing specified vmx was finished successfully.\n" +
                    "vmx file is '" + _vmx_file + "'.", EventLogEntryType.Information);
            }
        }

        protected void WriteLog(string message, EventLogEntryType type)
        {
            try
            {
                EventLog.WriteEntry(message, type);
            }
            finally
            {
            }
        }
    }
}
