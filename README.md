# WinDefExclusionFinder
Small Script that permits to enumerate folders in Windows Defender Exclusion List with no Administrative privileges

## Small Explanation of the command used
This small script is based on the following command that can be used to check if a specific folder is part of the Windows Defender Exclusion List. It uses an issue where Windows Defender will skip and notify that you that it did when starting a scan on a folder that is in the Exclusion list

"C:\Program Files\Windows Defender\MpCmdRun.exe" -Scan -ScanType 3 -File "C:\path\to\folder\\*""

![image](https://github.com/user-attachments/assets/e416a271-6127-4911-9b89-45a51de1cfe1)

As we can see here, when the folder is not part of the exclusion list we get "Failed with hr = 0x8050....", but when it is part of the Exclusion list we get an explicit log information saying the folder was skipped.

### Automatisation
Using our tool now we could check a folder and its children together without much issue.
  - Using our Published Self-Contained Release version
     ```c
     WinDefExclusionFinder.exe C:\path\to\folder\ 1
     ```
     This will check at one level of depth inside the target Folder
  - You can also use quotes for troublesome folders but make sure to use / instead of \ as it can cancel the quote:
     ```c
     WinDefExclusionFinder.exe "C:/path/to/folder/" 2
     ```

![WinDefExclusionFinder_demo](https://github.com/user-attachments/assets/b643a8d1-1597-427b-b4eb-fe9827df19a8)
