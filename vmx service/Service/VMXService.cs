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
        #region Fields

        protected readonly string _vmx_file = null;
        protected IVMController _control = null;
        protected bool _invalid_state = false;
        protected Timer _timer = null;

        #endregion

        #region Constructor

        public Service()
            : base()
        {
            SetServiceBehavior();
            _invalid_state = true;

            WriteLog(
                "Invalid arguments!\n{0}", EventLogEntryType.FailureAudit,
                String.Join("\n", Environment.GetCommandLineArgs()));
        }

        public Service(string vmx_file, VMWareInfo.VMCoreTypes vm_target)
            : base()
        {
            SetServiceBehavior();
            _vmx_file = vmx_file;

            try
            {
                if (IsUsingApi)
                {
                    _control = new VMControllerByAPI(vm_target);
                }
                else
                {
                    _control = new VMControllerByVMRun(vm_target);
                }
            }
            catch (VMXServiceException)
            {
                _invalid_state = true;
                WriteLog("Failed To Initialize VMWare controller!", EventLogEntryType.Error);
                return;
            }

            if (!File.Exists(_vmx_file))
            {
                WriteLog("Specified vmx file '{0}' does not exists!", EventLogEntryType.FailureAudit, _vmx_file);
                _invalid_state = true;
                return;
            }

            _timer = new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);
            _timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
            _timer.Enabled = false;
        }

        #endregion

        #region Properties

        protected virtual bool IsUsingApi
        {
            get
            {
                return true;
            }
        }

        protected void SetServiceBehavior()
        {
            AutoLog = false;
            CanPauseAndContinue = true;
            ServiceName = "vmx service";
        }

        #endregion

        #region Events

        protected void OnTimerEvent(object source, ElapsedEventArgs e)
        {
            if (_control == null)
            {
                WriteLog(
                    "Cannot control VMX currently.", EventLogEntryType.Error);
                return;
            }

            if (!_control.IsRunning(_vmx_file))
            {
                _invalid_state = true;
                WriteLog(
                    "The VMX should be running, but not.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Warning, _vmx_file);
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

            if (_control == null)
            {
                WriteLog(
                    "Cannot control VMX currently.", EventLogEntryType.Error);
                return;
            }


            if (_control.IsRunning(_vmx_file))
            {
                WriteLog(
                    "The VMX file is aleady running.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Information, _vmx_file);
            }
            else if (!_control.StartVMX(_vmx_file))
            {
                WriteLog(
                    "Starting specified vmx was failed.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Error, _vmx_file);
                Stop();
            }
            else
            {
                WriteLog(
                    "Now starting specified vmx.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Information, _vmx_file);
                _timer.Enabled = true;
            }
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;

            if (_invalid_state) return;

            if (_control == null)
            {
                WriteLog(
                    "Cannot control VMX currently.", EventLogEntryType.Error);
                return;
            }


            if (!_control.IsRunning(_vmx_file))
            {
                WriteLog(
                    "Specified vmx might not be running.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.FailureAudit, _vmx_file);
                return;
            }

            if (!_control.StopVMX(_vmx_file))
            {
                WriteLog(
                    "Stopping the vmx was failed.\n" + 
                    "vmx file is '{0}'.", EventLogEntryType.Error, _vmx_file);

            }
            else 
            {
                WriteLog(
                    "Stopping specified vmx was finished successfully.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Information, _vmx_file);
            }
        }

        protected override void OnPause()
        {
            if (_invalid_state) return;

            if (_control == null)
            {
                WriteLog(
                    "Cannot control VMX currently.", EventLogEntryType.Error);
                return;
            }


            if (!_control.PauseVMX(_vmx_file))
            {
                WriteLog(
                    "Pausing specified vmx was failed.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Error, _vmx_file);

            }
            else
            {
                WriteLog(
                    "Pausing specified vmx was finished successfully.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Information, _vmx_file);
            }
        }

        protected override void OnContinue()
        {
            if (_invalid_state) return;

            if (_control == null)
            {
                WriteLog(
                    "Cannot control VMX currently.", EventLogEntryType.Error);
                return;
            }


            if (!_control.ContinueVMX(_vmx_file))
            {
                WriteLog(
                    "Continuing specified vmx was failed.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Error, _vmx_file);

            }
            else
            {
                WriteLog(
                    "Continuing specified vmx was finished successfully.\n" +
                    "vmx file is '{0}'.", EventLogEntryType.Information);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (_control != null)
            {
                _control.Dispose();
                _control = null;
            }
            base.Dispose(disposing);
        }

        #region Logs

        protected void WriteLog(string message, EventLogEntryType type, params object[] messageArgs)
        {
            try
            {
                EventLog.WriteEntry(string.Format(message, messageArgs), type);
            }
            finally { }
        }

        protected void WriteLog(string message, EventLogEntryType type)
        {
            try
            {
                EventLog.WriteEntry(message, type);
            }
            finally { }
        }

        #endregion
    }
}
