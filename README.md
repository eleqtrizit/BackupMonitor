# Backup Monitor

## Monitor Files

Simple program to help monitor the presence of files made within the past 24 hrs.  Originally meant to quickly help me verify the presence of backups.  The program will then email the administrator the results of the search.  The program must be manually entered into the Task Scheduler to work.

## Future Improvements
Currently there is no way to send through SMTP with authentication, which is kind of huge.

The program should also check size of the files, to make sure they are not empty.

## How to Use
There are two sample config files included in the project, BackupCheck.cfg and BackupCheckList.cfg.  

### BackupCheck.cfg
to=adminalerts@yourcompany.com<br>
from=backupCheck@yourcompany.com<br>
mailserver=mail.yourcompany.com<br>
checklist=c:\BackupMonitor\BackupCheckList.cfg

### BackupCheckList.cfg
Z:\Backups\Servers\AD\c	*
Z:\LinuxStaging\image1\home\backups\mysql	*dump.gz

First one means check that path on Z: for _any_ file.
The seconds one means check that path on Z: for files ending in dump.gz

