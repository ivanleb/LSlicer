using PluginFramework.Core;
using PluginFramework.Core.Configuration;
using PluginFramework.Core.Loading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginFramework.Implementations.Loading
{
    public class ReflectionPlugnLoader : IPluginLoader
    {
        private readonly IPluginConfigStorage _pluginConfigStorage;
        private readonly LoadContextContainer _loadContextContainer;
        private readonly IPluginChecker _pluginChecker;
        private readonly IPluginInstancesCreator<IPlugin> _pluginCreator;
        private readonly PluginGlobalState _pluginGlobalState;

        public ReflectionPlugnLoader(IPluginConfigStorage pluginConfigStorage, LoadContextContainer appDomainContainer, IPluginChecker pluginChecker, IPluginInstancesCreator<IPlugin> pluginCreator, PluginGlobalState pluginGlobalState)
        {
            _pluginConfigStorage = pluginConfigStorage;
            _loadContextContainer = appDomainContainer;
            _pluginChecker = pluginChecker;
            _pluginCreator = pluginCreator;
            _pluginGlobalState = pluginGlobalState;
        }

        public PluginConfig[] LoadPluginFromDirectory(DirectoryInfo pluginDirInfo)
        {
            PluginConfig[] configs = _pluginConfigStorage.ReadPluginConfigs(pluginDirInfo);

            _pluginChecker.CheckLoadedPluginCount(configs.Length);

            string assemblyName = Path.Combine(pluginDirInfo.FullName, configs.First().AssemblyName + ".dll");
            Assembly assembly = _loadContextContainer.LoadAssembly(assemblyName);

            IReadOnlyList<IPlugin> plugins = _pluginCreator.CreateInstances(assembly, typeof(IPlugin));

            _pluginChecker.CheckLoadedPluginCount(plugins.Count());

            foreach (IPlugin plugin in plugins)
            {
                _pluginGlobalState.AddPlugin(plugin);
            }

            return configs;
        }
    }
}
