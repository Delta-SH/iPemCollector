echo off
echo ****************************
echo ��װTaskService����
echo ****************************
pause
cd /d "%~dp0"
echo *���ڰ�װ����. . .
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil /LogFile= TaskService.exe > InstallService.log
echo *����װ���
echo *������������. . .
sc failure "TaskService" reset= 86400 actions= restart/30000 >> InstallService.log
sc start "TaskService" >> InstallService.log
echo *�����������
echo *��ϸ��־�鿴InstallService.log
pause

