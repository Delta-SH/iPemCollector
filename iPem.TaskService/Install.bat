echo off
echo ****************************
echo ��װPECS-II Data Service
echo ****************************
pause
cd /d "%~dp0"
echo *���ڰ�װ����. . .
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil /LogFile= TaskService.exe > InstallService.log
echo *����װ���
echo *������������. . .
sc failure "PECS2Service" reset= 86400 actions= restart/30000 >> InstallService.log
sc start "PECS2Service" >> InstallService.log
echo *�����������
echo *��ϸ��־�鿴InstallService.log
pause

