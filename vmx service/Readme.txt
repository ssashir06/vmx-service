////////////////////////////////////////////////////
//////          VMX_Service readme         /////////
////////////////////////////////////////////////////

1. About this software
 This software is a Windows Service to start some virtual machines which is created in VMWare background automatically.
 It is designed for working with VMWare Workstation or VMWare Player.

1.1 Why is this software needed?
 This software is needed to run some virtual machines with VMWare Workstation or VMWare Player like being under VMWare Server.

1.2 Why don't use VMWare Server?
 Because its GUI's response is too slow.

2. Requested system equipment
 There are some requestments to use this software.
 Please install them before setting up this software :

A. VMWare Workstation or VMWare Player
	... to run vmx files as a virtual server.
B. Microsoft .Net Framework 2.0 or heigher
	... to install/run our executable files.

3. How to set up / How to remove?
3.1 Windows XP or older
3.1.1 Setting up

A. Edit "install.bat"
   Change VMXNAME and VMXFILE.
   VMXNAME is a name of the service, like /^[a-zA-Z0-9_-]+$/.
   VMXFILE is a fullpath of the vmx file to run, like "C:\Path\To\VMX.vmx".
B. Run "install.bat" as a administrator.
   The message "The Commit phase completed successfully." means succeess.
   The message "The Rollback phase completed successfully." means failure.
C. Restart your computer.
D. Done.

3.1.2 Remove

A. Edit "uninstall.bat"
   Change VMXNAME.
   VMXNAME is a name of the service, you specified on the "install.bat".
B. Run "uninstall.bat" as a administrator.
   The message "The Commit phase completed successfully." means succeess.
   The message "The Rollback phase completed successfully." means failure.
C. Restart your computer.
D. Done.


4. Others

 Have a nice day.