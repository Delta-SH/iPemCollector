echo off
echo ****************************
echo ж��TaskService����
echo ****************************
pause
cd /d "%~dp0"
echo *����ֹͣ����. . .
sc stop "TaskService" > InstallService.log
echo *������ֹͣ
echo *����ж�ط���. . .
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil /LogFile= /u TaskService.exe >> InstallService.log
echo *����ж�����
echo *��ϸ��־�鿴InstallService.log
pause