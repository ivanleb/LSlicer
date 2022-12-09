using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WixSharp;
using Condition = WixSharp.Condition;

//#error "DON'T FORGET to install NuGet package 'WixSharp' and remove this `#error` statement."
// NuGet console: Install-Package WixSharp
// NuGet Manager UI: browse tab
//https://github.com/oleg-shilo/wixsharp/wiki/Building-MSI-%E2%80%93-Step-by-step-tutorial

namespace SlicerInstaller
{
    class Program
    {
        static void Main()
        {
            string mainAppDataFolder = "LSlicer";
            string releaseFolder = "..\\LSlicer\\bin\\x64\\Release\\";
            string installDir = @"%ProgramFiles%\ESTO\Slicer\";

            ConfigFileModifier.ApplyMapping("mapping.txt", releaseFolder + "LSlicer.exe.config");

            DirectoryInfo directory = new DirectoryInfo(releaseFolder);

            List<WixEntity> files = directory
                .GetFiles("*")
                .Where(f => IsNeedToInstallFile(f))
                .Select(f => new WixSharp.File(f.FullName))
                .Cast<WixEntity>()
                .ToList();

            directory.GetDirectories()
                .Where(d => IsPlatformSpecificName(d))
                .Select(d => new Dir(d.Name, d.GetFiles()
                                                  .Select(f => new WixSharp.File(f.FullName))
                                                  .ToArray()))
                .Cast<WixEntity>()
                .ForEach(d => files.Add(d));

            AddShortCuts(releaseFolder, files);
            AddAppDataFoldersAndFiles(mainAppDataFolder, releaseFolder, files);

            var project = new Project("LASlicer",
                              new Dir(installDir, files.ToArray())
                                );

            project.GUID = new Guid("d5e752e7-bed7-4ea2-8f9f-d61cc0d526ad");
            project.LicenceFile = Path.Combine(releaseFolder, "Resources","Licence.rtf");
            project.UpgradeCode = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.Encoding = System.Text.Encoding.UTF8;
            project.UI = WUI.WixUI_Minimal;
            project.OutFileName = "LSlicer";
            project.BuildMsi(); 
        }

        private static bool IsPlatformSpecificName(DirectoryInfo d)
        {
            return d.Name.Contains("x64") || d.Name.Contains("x86");
        }

        private static bool IsNeedToInstallFile(FileInfo f)
        {
            return (f.FullName.Contains("exe")
                    || f.FullName.Contains("dll")
                    || f.FullName.Contains("config")
                    || f.FullName.Contains("db"))
                    && f.Name != "LSlicer.exe";
        }

        private static void AddShortCuts(string releaseFolder, List<WixEntity> files)
        {
            WixEntity DesktopShortcut = new WixSharp.File(releaseFolder + "LSlicer.exe",
                                        new FileShortcut("LSlicer", "INSTALLDIR"), //INSTALLDIR is the ID of "%ProgramFiles%\My Company\My Product"
                                        new FileShortcut("LSlicer", @"%Desktop%") { IconFile = $"{releaseFolder}//Resources//ApplicationIcon.ico" });
            files.Add(DesktopShortcut);
        }

        private static void AddAppDataFoldersAndFiles(string mainAppDataFolder, string releaseFolder, List<WixEntity> files)
        {
            var directory = new DirectoryInfo($"{releaseFolder}//Resources//");
            var settingsFiles = directory.GetFiles("*.json").Select(f => new WixSharp.File(f.FullName)).ToArray();
            Dir resourceDir = new Dir("Resources", settingsFiles);
            Dir jobsDir = new Dir("Jobs");
            Dir tempDir = new Dir("Temp");
            Dir logsDir = new Dir("Logs");

            Dir mainAppDataDir = new Dir(mainAppDataFolder, resourceDir, jobsDir, tempDir, logsDir);
            WixEntity resourcesDir = new Dir(@"%AppData%", mainAppDataDir);
            files.Add(resourcesDir);
        }
    }
}