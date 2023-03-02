using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginFramework.CustomPlugin.Helpers;

namespace PluginFramework.CustomPlugin.Installation
{
    public class ReflectionPluginInstaller : IInstallPluginStrategy, IPluginsActivator
    {
        private bool _isPluginsInitialized = false;

        public void Install(string pluginPackagePath)
        {
            FileInfo pluginPackageFileInfo = new FileInfo(pluginPackagePath);

            if (!pluginPackageFileInfo.Exists)
                throw new FileNotFoundException(pluginPackageFileInfo.FullName);

            string pluginName = Path.GetFileNameWithoutExtension(pluginPackageFileInfo.FullName);

            IEnumerable<string> pluginNames = PluginGlobalState.DomainContainer.GetAllDomainNames();
            if (pluginNames.Contains(pluginName))
                throw new ArgumentException($"Plugin with name {pluginName} already installed");

            string pluginDirectory = FileHelper.GetPluginDirectoryByName(pluginName);

            DirectoryInfo pluginDirInfo = Directory.CreateDirectory(pluginDirectory);

            ZipHelper.ExtractZipFile(pluginPackageFileInfo.FullName, password: "", pluginDirInfo.FullName);

            PluginConfig[] configs = LoadPluginFromDirectory(pluginDirInfo);

            ConfigHelper.AddInstalledPluginToDescription(configs);
        }

        private static PluginConfig[] LoadPluginFromDirectory(string pluginDirPath)
            => LoadPluginFromDirectory(new DirectoryInfo(pluginDirPath));

        private static PluginConfig[] LoadPluginFromDirectory(DirectoryInfo pluginDirInfo)
        {
            PluginConfig[] configs = ConfigHelper.ReadPluginDescriptions(pluginDirInfo);

            GuardMethod(configs.Length);

            string assemblyName = Path.Combine(pluginDirInfo.FullName, configs.First().AssemblyName + ".dll");
            Assembly assembly = PluginGlobalState.DomainContainer.LoadDomain(assemblyName);

            IEnumerable<IPlugin> plugins = ReflectionHelper.CreateInstances<IPlugin>(assembly);

            GuardMethod(plugins.Count());

            foreach (IPlugin plugin in plugins)
            {
                PluginGlobalState.AddPlugin(plugin);
            }

            return configs;
        }

        public void MakePluginPackage(string pluginDirectoryPath, string targetDirectoryPath)
        {
            if (!Directory.Exists(pluginDirectoryPath))
                throw new DirectoryNotFoundException(pluginDirectoryPath);

            if (!Directory.Exists(targetDirectoryPath))
                throw new DirectoryNotFoundException(targetDirectoryPath);

            IEnumerable<Type> pluginTypes = ReflectionHelper.GetTypesFormAssemblies<IPlugin>(pluginDirectoryPath);

            (List<PluginConfig> pluginConfigs, List<Assembly> assembliesWithPlugins) = GetConfigsAndAssemblies(pluginTypes);

            GuardMethod(pluginConfigs.Count);
            GuardMethod(assembliesWithPlugins.Count);

            string path = ReflectionHelper.GetDomainName(assembliesWithPlugins.First().GetName());
            DirectoryInfo directoryToPack = FileHelper.PrerareDirectory(targetDirectoryPath, path);
            FileHelper.CopyFilesRecursively(pluginDirectoryPath, directoryToPack.FullName);
            ConfigHelper.WriteXmlDescription(pluginConfigs, directoryToPack);
            ZipHelper.ZipFolder(directoryToPack.FullName + ".zip", "", directoryToPack.FullName);
            Directory.Delete(directoryToPack.FullName, recursive: true);
            Process.Start(targetDirectoryPath);
        }

        public void Uninstall(IPlugin plugin)
        {
            PluginGlobalState.RemovePlugin(plugin);
            ConfigHelper.RemovePluginFromDescriptions(plugin);
            FileHelper.RemoveDirectory(plugin);
        }

        public void LoadInstalledPlugins()
        {
            if (!_isPluginsInitialized)
            {
                LoadInstalledPluginsImpl();
                _isPluginsInitialized = true;
            }
        }

        private static void GuardMethod(int plugincount)
        {
            if (plugincount > 1)
                throw new ApplicationException("Should be only one plugin");
            if (plugincount < 1)
                throw new ApplicationException("No plugins in assembly");
        }

        private void LoadInstalledPluginsImpl()
        {
            string plugPath = ConfigHelper.GetInstalledPluginListPath();
            var configs = ConfigHelper.GetInstalledPluginsFromDescription(plugPath);

            List<Exception> exceptions = new List<Exception>();
            foreach (PluginConfig config in configs)
            {
                try
                {
                    string plugDir = FileHelper.GetPluginDirectoryByName(config.PluginName);
                    LoadPluginFromDirectory(plugDir);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Any())
                throw new AggregateException("Some plugin(s) could not be loaded", exceptions.ToArray());
        }

        private static (List<PluginConfig> pluginConfigs, List<Assembly> assembliesWithPlugins) GetConfigsAndAssemblies(IEnumerable<Type> types)
        {
            var assembliesWithPlugins = new List<Assembly>();
            var pluginConfigs = new List<PluginConfig>();

            foreach (var pluginType in types)
            {
                var assemblyName = pluginType.Assembly.GetName();
                PluginConfig pluginConfig = new PluginConfig()
                {
                    Name = pluginType.Name,
                    AssemblyName = assemblyName.Name,
                    Namespace = pluginType.Namespace,
                    PluginName = ReflectionHelper.GetDomainName(assemblyName)
                };
                pluginConfigs.Add(pluginConfig);
                assembliesWithPlugins.Add(pluginType.Assembly);
            }

            return (pluginConfigs, assembliesWithPlugins);
        }
    }
}
