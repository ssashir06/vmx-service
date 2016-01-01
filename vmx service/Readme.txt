////////////////////////////////////////////////////
//////          VMX_Service readme         /////////
////////////////////////////////////////////////////

1. Setting up

A. Edit "install.bat"
   Change VMXNAME and VMXFILE.
   VMXNAME is a name of the service, like /^[a-zA-Z0-9_-]+$/.
   VMXFILE is a fullpath of the vmx file to run, like "C:\Path\To\VMX.vmx".
B. Run "install.bat" as a administrator.
   The message "The Commit phase completed successfully." means succeess.
   The message "The Rollback phase completed successfully." means failure.
C. Restart your computer.
D. Done.


2. Remove

A. Edit "uninstall.bat"
   Change VMXNAME.
   VMXNAME is a name of the service, you specified on the "install.bat".
B. Run "uninstall.bat" as a administrator.
   The message "The Commit phase completed successfully." means succeess.
   The message "The Rollback phase completed successfully." means failure.
C. Restart your computer.
D. Done.

