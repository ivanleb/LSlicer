using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginFramework.Core;
using PluginFramework.Core.Configuration;
using PluginFramework.Core.Installation;
using PluginFramework.Core.Loading;
using PluginFramework.CustomPlugin.Helpers;
using PluginFramework.CustomPlugin.Zipping;
using PluginFramework.Implementations;

namespace PluginFramework.CustomPlugin.Installation
{
    public class ReflectionPluginInstaller : IInstallPluginStrategy
    {
        private readonly IPluginConfigStorage _pluginConfigStorage;
        private readonly IZipper _zipper;
        private readonly IPluginLoader _pluginLoader;
        private readonly IPluginChecker _pluginChecker;
        private readonly PluginGlobalState _pluginGlobalState;

        public ReflectionPluginInstaller(IPluginConfigStorage pluginConfigStorage, IZipper zipper, IPluginLoader pluginLoader, IPluginChecker pluginChecker, PluginGlobalState pluginGlobalState)
        {
            _pluginConfigStorage = pluginConfigStorage;
            _zipper = zipper;
            _pluginLoader = pluginLoader;
            _pluginChecker = pluginChecker;
            _pluginGlobalState = pluginGlobalState;
        }

        public void Install(string pluginPackagePath)
        {
            FileInfo pluginPackageFileInfo = TryGetPluginFileInfo(pluginPackagePath);

            string pluginName = Path.GetFileNameWithoutExtension(pluginPackageFileInfo.FullName);

            CheckIfPluginAlreadyInstalled(pluginName);

            DirectoryInfo pluginDirInfo = GetPluginDirectoryByPluginName(pluginName);

            _zipper.ExtractZipFile(pluginPackageFileInfo.FullName, password: "", pluginDirInfo.FullName);

            PluginConfig[] configs = _pluginLoader.LoadPluginFromDirectory(pluginDirInfo);

            _pluginConfigStorage.AddInstalledPluginToConfigStorage(configs);
        }

        public void MakePluginPackage(string pluginDirectoryPath, string targetDirectoryPath)
        {
            CheckIfDirectoryExists(pluginDirectoryPath);
            CheckIfDirectoryExists(targetDirectoryPath);

            TryGetConfigsAndTheirAssemblies(pluginDirectoryPath, out List<PluginConfig> pluginConfigs, out List<Assembly> assembliesWithPlugins);

            PreparePackage(pluginDirectoryPath, targetDirectoryPath, pluginConfigs, assembliesWithPlugins);

            OpenDirectoryInExplorer(targetDirectoryPath);
        }

        public void Uninstall(IPlugin plugin)
        {
            _pluginGlobalState.RemovePlugin(plugin);
            _pluginConfigStorage.RemovePluginFromDescriptions(plugin);
            FileHelper.RemoveDirectory(plugin);
        }

        private static DirectoryInfo GetPluginDirectoryByPluginName(string pluginName)
        {
            string pluginDirectory = FileHelper.GetPluginDirectoryByName(pluginName);
            return Directory.CreateDirectory(pluginDirectory);
        }

        private void CheckIfPluginAlreadyInstalled(string pluginName)
        {
            IEnumerable<string> pluginNames = _pluginGlobalState.AssemblyContainer.GetAllDomainNames();
            if (pluginNames.Contains(pluginName))
                throw new ArgumentException($"Plugin with name {pluginName} already installed");
        }

        private static FileInfo TryGetPluginFileInfo(string pluginPackagePath)
        {
            FileInfo pluginPackageFileInfo = new FileInfo(pluginPackagePath);

            if (!pluginPackageFileInfo.Exists)
                throw new FileNotFoundException(pluginPackageFileInfo.FullName);
            return pluginPackageFileInfo;
        }

        private void TryGetConfigsAndTheirAssemblies(string pluginDirectoryPath, out List<PluginConfig> pluginConfigs, out List<Assembly> assembliesWithPlugins)
        {
            IEnumerable<Type> pluginTypes = ReflectionHelper.GetTypesFormAssemblies<IPlugin>(pluginDirectoryPath);

            (pluginConfigs, assembliesWithPlugins) = ReflectionPluginInstallerHelpers.GetConfigsAndAssemblies(pluginTypes);
            _pluginChecker.CheckLoadedPluginCount(pluginConfigs.Count);
            _pluginChecker.CheckLoadedPluginCount(assembliesWithPlugins.Count);
        }

        private static void CheckIfDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(directoryPath);
        }

        private static void OpenDirectoryInExplorer(string targetDirectoryPath)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = targetDirectoryPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void PreparePackage(string pluginDirectoryPath, string targetDirectoryPath, List<PluginConfig> pluginConfigs, List<Assembly> assembliesWithPlugins)
        {
            string path = ReflectionHelper.GeAssemblyName(assembliesWithPlugins.First().GetName());
            DirectoryInfo directoryToPack = FileHelper.PrerareDirectory(targetDirectoryPath, path);
            FileHelper.CopyFilesRecursively(pluginDirectoryPath, directoryToPack.FullName);
            _pluginConfigStorage.CreatePluginConfig(pluginConfigs, directoryToPack);
            _zipper.ZipFolder(directoryToPack.FullName + ".zip", "", directoryToPack.FullName);
            Directory.Delete(directoryToPack.FullName, recursive: true);
        }
    }
}
