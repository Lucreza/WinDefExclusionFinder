# WinDefExclusionFinder
Small Script that permits to enumerate folders in Windows Defender Exclusion List with no Administrative privileges

#
This small script is based on the following command that can be used to check if a specific folder is part of the Windows Defender Exclusion List.

"C:\Program Files\Windows Defender\MpCmdRun.exe" -Scan -ScanType 3 -File "C:\path\to\folder\*""
