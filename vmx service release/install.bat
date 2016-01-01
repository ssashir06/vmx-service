:: これはvmx serviceをインストールするためのバッチです。管理者として実行してください。
:: This is a batch file for install vmx service. Please run as administrator.

@echo off

set VMXNAME=MyUbuntu1
set VMXFILE=C:\path\to\your.vmx
set ACCOUNTNAME=%USERDOMAIN%\%USERNAME%

echo Installing vmx service ...

if not exist "%VMXFILE%" (
echo Cannot find VmxFile: %VmxFile%
pause
exit /b
)

echo VmxName = %VMXNAME%
echo VmxFile = %VMXFILE%
echo AccountName = %ACCOUNTNAME%
pause

cd %~dp0
set INSTALLUTIL=C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil
%INSTALLUTIL% /VMXName="%VMXNAME%" /VmxFile="%VMXFILE%" /AccountName="%ACCOUNTNAME%" ".\vmx service.exe"

pause