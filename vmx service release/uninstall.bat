:: �����vmx service���A���C���X�g�[�����邽�߂̃o�b�`�ł��B�Ǘ��҂Ƃ��Ď��s���Ă��������B
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