echo off
echo ****************************
echo 安装PECS-II Data Service
echo ****************************
pause
cd /d "%~dp0"
echo *正在安装服务. . .
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil /LogFile= TaskService.exe > InstallService.log
echo *服务安装完成
echo *正在启动服务. . .
sc failure "PECS2Service" reset= 86400 actions= restart/30000 >> InstallService.log
sc start "PECS2Service" >> InstallService.log
echo *服务启动完成
echo *详细日志查看InstallService.log
pause

