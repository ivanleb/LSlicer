using PluginFramework.Core;
using PluginFramework.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginFramework.CustomPlugin.Helpers
{
    public static class FileHelper
    {
        public static FileToRemoveContainer FilesToRemove = new FileToRemoveContainer();
        public static bool DeleteFilesAfterClosingApp = false;

        internal static void RemoveDirectory(string pluginDirectory)
        {
            if (Directory.Exists(pluginDirectory))
            {
                try
                {
                    Directory.Delete(pluginDirectory, recursive: true);
                }
                catch (UnauthorizedAccessException e)
                {
                    string localPath = e.Message.Split('\"').ElementAt(1);
                    string fullPath = Path.Combine(pluginDirectory, localPath);
                    if (File.Exists(fullPath) && DeleteFilesAfterClosingApp)
                        FilesToRemove.Add(fullPath);
                }
            }
        }

        internal static void RemoveDirectory(IPlugin plugin)
        {
            string directory = GetPluginDirectoryByName(plugin.GetType().Name);
            RemoveDirectory(directory);
        }

        internal static List<string> CopyFilesToDirectory(string sourceDirectoryPath, string targetDirectoryPath)
        {
            List<string> fileList = new List<string>();
            foreach (FileInfo sourceFileInfo in new DirectoryInfo(sourceDirectoryPath).GetFiles())
            {
                string destFilePath = Path.Combine(targetDirectoryPath, sourceFileInfo.Name);
                if (IsNeedToCopyFile(destFilePath))
                {
                    File.Copy(sourceFileInfo.FullName, destFilePath);
                    fileList.Add(destFilePath);
                }
            }

            return fileList;
        }

        internal static bool IsNeedToCopyFile(string destFilePath)
        {
            return !File.Exists(destFilePath)
                && !destFilePath.Contains(PluginValues.DescriptionFileName)
                && !destFilePath.Contains(PluginValues.UninstallInfoFileName);
        }

        internal static void RemoveFiles(List<string> files)
        {
            FilesToRemove.AddRange(files);
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            try
            {
                FilesToRemove.RemoveAll();
            }
            catch (Exception)
            {
            }
        }

        internal static string GetPluginDirectoryByName(string pluginName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginValues.PluginsDirectoryName, pluginName);
        }

        internal static void WriteCopiedFileListForUninstall(List<string> copiedFilesList, string pluginDirectoryPath)
        {
            using (TextWriter writer = new StreamWriter(File.Open(Path.Combine(pluginDirectoryPath, PluginValues.UninstallInfoFileName), FileMode.OpenOrCreate)))
            {
                foreach (var fileName in copiedFilesList)
                {
                    writer.WriteLine(fileName);
                }
            }
        }

        internal static List<string> GetPluginFilesForUninstall(string pluginDirectoryPath)
        {
            List<string> files = new List<string>();
            using (TextReader reader = new StreamReader(File.OpenRead(Path.Combine(pluginDirectoryPath, PluginValues.UninstallInfoFileName))))
            {
                string fileName;
                while ((fileName = reader.ReadLine()) != null)
                {
                    if (File.Exists(fileName))
                        files.Add(fileName);
                }
            }
            return files;
        }

        internal static DirectoryInfo PrerareDirectory(string targetDirectoryPath, string pluginName)
        {
            string pluginDirectory = Path.Combine(targetDirectoryPath, pluginName);
            if (Directory.Exists(pluginDirectory))
                Directory.Delete(pluginDirectory);

            return Directory.CreateDirectory(pluginDirectory);
        }

        internal static void CopyAssemblies(List<Assembly> assembliesWithPlugins, DirectoryInfo resultDir)
        {
            foreach (var ass in assembliesWithPlugins)
            {
                string copiedFilePath = Path.Combine(resultDir.FullName, Path.GetFileName(ass.Location));
                if (!File.Exists(copiedFilePath))
                    File.Copy(ass.Location, copiedFilePath);
            }
        }
        internal static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        internal static string GetSubDirectoryPath(string path)
            => Path.GetDirectoryName(path).Replace(AppDomain.CurrentDomain.BaseDirectory, "");
    }
}
