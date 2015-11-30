
////////////////////////////////////////////////////
//////          VMX_Service readme         /////////
////////////////////////////////////////////////////

vmx-service
===
This is a Windows Service running Vmware Workstation 7, supports Windows XP and Windows 7.


About this software
---

 This software is a Windows Service to start some virtual machines which is created in VMWare background automatically.
 It is designed for working with VMWare Workstation or VMWare Player.

Why is this software needed?
---

 This software is needed to run some virtual machines with VMWare Workstation or VMWare Player like being under VMWare Server.

Why don't use VMWare Server?
---

 Because its GUI's response is too slow.

Requested system equipment
---

 There are some requestments to use this software.
 Please install them before setting up this software :

1. VMWare Workstation or VMWare Player
	... to run vmx files as a virtual server.
2. Microsoft .Net Framework 2.0 or heigher
	... to install/run our executable files.

How to set up / How to remove
===
### Windows XP or older
#### Setting up

1. Login as a administrator account.
2. Determine the full-path to the vmx file of your virtual machine (ex. "C:\Path\To\VMX.vmx").
3. Move the directory "vmx service release" to some place (ex. "C:\vmx service release").
4. Start "Command Prompt" from your taskbar.
5. Type:
>	CD %windir%\Microsoft.NET\Framework\v4.0.30319
6. Type:
>	Installutil /Name="MyServer1" /Vmx="C:\Path\To\VMX.vmx" "C:\vmx service release\vmx service.exe"
7. Restart your computer.
8. Done.

##### Warnings:
* Step 6:
  *	The name "MyServer1" is just an example. It is can be like /^[a-zA-Z_-]+$/.
* Step 6:
  *	Double quotations are required.
* Step 6:
  *	The message "The Commit phase completed successfully." means succeess.
  *	The message "The Rollback phase completed successfully." means failure.

#### Remove

1. Login as a administrator account.
2. Determine the full-path to the directory this software is installed (ex. "C:\vmx service release\vmx service.exe").
3. Start "Command Prompt" from your taskbar.
4. Type:
>	CD %windir%\Microsoft.NET\Framework\v4.0.30319
5. Type:
>	Installutil /u /Name="MyServer1" "C:\vmx service release\vmx service.exe"
6. Restart your computer.
7. Done.

### Windows Vista or Windows 7 or later
#### Setting up

See Setting up section and you have to start "Command Prompt" as a Administrator in the Step 4.
To do this, Right click "Command Prompt" and click "Run as administrator".

#### Remove

See Remove section and also you have to start "Command Prompt" as a Administrator in the Step 3.


Others
---

This application was developed around 2009, thus it may doesn't work recent environment. 😢
Someone please help?
