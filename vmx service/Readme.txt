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

A. Login as a administrator account.
B. Determine the full-path to the vmx file of your virtual machine (ex. "C:\Path\To\VMX.vmx").
C. Move the directory "vmx service release" to some place (ex. "C:\vmx service release").
D. Start "Command Prompt" from your taskbar.
E. Type:
	CD %windir%\Microsoft.NET\Framework\v4.0.30319
F. Type:
	Installutil /Name="MyServer1" /Vmx="C:\Path\To\VMX.vmx" "C:\vmx service release\vmx service.exe"
G. Restart your computer.
H. Done.

Warnings:
Step F:
	The name "MyServer1" is just an example. It is can be like /^[a-zA-Z_-]+$/.
Step F:
	Double quotations are required.
Step F:
	The message "The Commit phase completed successfully." means succeess.
	The message "The Rollback phase completed successfully." means failure.

3.1.2 Remove

A. Login as a administrator account.
B. Determine the full-path to the directory this software is installed (ex. "C:\vmx service release\vmx service.exe").
C. Start "Command Prompt" from your taskbar.
D. Type:
	CD %windir%\Microsoft.NET\Framework\v4.0.30319
E. Type:
	Installutil /u /Name="MyServer1" "C:\vmx service release\vmx service.exe"
F. Restart your computer.
G. Done.

3.2. Windows Vista or Windows 7 or later
3.2.1 Setting up

See 3.1.1 and you have to start "Command Prompt" as a Administrator in the Step D.
To do this, Right click "Command Prompt" and click "Run as administrator".

3.2.2 Remove

See 3.1.2 and also you have to start "Command Prompt" as a Administrator in the Step C.


4. Others

Go to website http://hujiko.net/ .
