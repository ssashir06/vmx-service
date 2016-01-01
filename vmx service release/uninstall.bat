:: これはvmx serviceをアンインストールするためのバッチです。管理者として実行してください。
:: This is a batch file for uninstall vmx service. Please run as administrator.

@echo off

set VMXNAME=UbuntuTest2

echo Uninstalling vmx service ...
echo VmxName = %VMXNAME%
pause

cd %~dp0
set INSTALLUTIL=C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil
%INSTALLUTIL% /u /VMXName="%VMXNAME%" ".\vmx service.exe"

pause