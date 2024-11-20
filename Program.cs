using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WindowsDefenderExclusionChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validate command-line arguments
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage: WinDefExclusionFinder.exe <RootFolderPath> [<Depth> (Default:0)]");
                return;
            }

            // Parse the arguments
            string rootFolderPath = args[0];
            int depth = 0;

            if (args.Length == 2)
            {
                // If depth argument is provided, try parsing it
                if (!int.TryParse(args[1], out depth) || depth < 0)
                {
                    Console.WriteLine("Please provide a valid depth (non-negative integer).");
                    Console.WriteLine("Usage: WinDefExclusionFinder.exe.exe <RootFolderPath> [<Depth> (Default:0)]");
                    return;
                }
            }
            depth = depth + 1;

            // Set to track exclusion folders
            HashSet<string> exclusionFolders = new HashSet<string>();

            // Scan the folder and all its subfolders up to the specified depth
            ScanFolder(rootFolderPath, exclusionFolders, depth, 0);

            // Output the exclusion list
            Console.WriteLine("\nExclusion Folders:");
            foreach (var exclusion in exclusionFolders)
            {
                Console.WriteLine(exclusion);
            }
        }

        static void ScanFolder(string folderPath, HashSet<string> exclusionFolders, int maxDepth, int currentDepth)
        {
            // If we have reached the maximum depth, stop scanning deeper
            if (currentDepth >= maxDepth)
            {
                return;
            }

            if (exclusionFolders.Contains(folderPath))
            {
                Console.WriteLine($"Skipping excluded folder: {folderPath}");
                return; // If the folder is already excluded, skip scanning it
            }

            // Log the folder being scanned
            Console.WriteLine($"Scanning folder: {folderPath}");

            // Escape the folder path properly for the command
            string escapedFolderPath = $"{folderPath}\\";
            // Build the full command
            string command = $"\"\"C:\\Program Files\\Windows Defender\\MpCmdRun.exe\" -Scan -ScanType 3 -File \"{escapedFolderPath}*\"\"";

            // Debugging: Print the full command to be executed
            //Console.WriteLine($"Executing command: {command}");

            string output = ExecuteCommand(command);

            // Log the output to troubleshoot
            //Console.WriteLine($"Command Output for {folderPath}:");
            //Console.WriteLine(output);

            // Check if the folder is skipped (excluded)
            if (output.Contains("was skipped"))
            {
                Console.WriteLine($"Folder {folderPath} was skipped by Windows Defender, adding to exclusion list.");
                exclusionFolders.Add(folderPath);
                return; // No need to scan subfolders for this folder
            }

            // If folder is not skipped, continue scanning its subfolders (go one level deeper)
            try
            {
                foreach (var subfolder in Directory.GetDirectories(folderPath))
                {
                    ScanFolder(subfolder, exclusionFolders, maxDepth, currentDepth + 1);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log the access denied error and continue with the next folder
                Console.WriteLine($"Access to folder {folderPath} is denied. Skipping this folder.");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static string ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true, // Capture error output as well
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas" // This will prompt for administrator rights
                };

                Process process = Process.Start(startInfo);
                using (StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd(); // Capture command output
                    string errorOutput = process.StandardError.ReadToEnd(); // Capture error output
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        Console.WriteLine("Error Output:");
                        Console.WriteLine(errorOutput);
                    }
                    return output;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
