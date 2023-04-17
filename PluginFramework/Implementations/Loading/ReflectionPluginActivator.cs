using PluginFramework.Core.Configuration;
using PluginFramework.Core.Loading;
using PluginFramework.CustomPlugin.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluginFramework.Implementations.Loading
{
    public class ReflectionPluginActivator : IPluginsActivator
    {
        private readonly IPluginConfigStorage _pluginConfigStorage;
        private readonly IPluginLoader _pluginLoader;
        private bool _isPluginsInitialized = false;

        public ReflectionPluginActivator(IPluginConfigStorage pluginConfigStorage, IPluginLoader pluginLoader)
        {
            _pluginConfigStorage = pluginConfigStorage;
            _pluginLoader = pluginLoader;
        }

        public void LoadInstalledPlugins()
        {
            if (!_isPluginsInitialized)
            {
                LoadInstalledPluginsImpl();
                _isPluginsInitialized = true;
            }
        }

        private void LoadInstalledPluginsImpl()
        {
            PluginConfig[] configs = _pluginConfigStorage.GetInstalledPluginsFromDescription();

            List<Exception> exceptions = new List<Exception>();
            foreach (PluginConfig config in configs)
            {
                try
                {
                    string plugDir = FileHelper.GetPluginDirectoryByName(config.PluginName);
                    _pluginLoader.LoadPluginFromDirectory(new DirectoryInfo(plugDir));
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Any())
                throw new AggregateException("Some plugin(s) could not be loaded", exceptions.ToArray());
        }
    }
}
