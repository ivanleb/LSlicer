using PluginFramework.Core;
using PluginFramework.Core.Installation;
using PluginFramework.Core.Loading;
using System.Collections.Generic;

namespace PluginFramework.Implementations
{
    public class PluginManager : IPluginManager
    {
        private readonly IInstallPluginStrategy _installStrategy;
        private readonly IPluginsActivator _activator;
        private readonly PluginGlobalState _pluginGlobalState;

        public PluginManager(IInstallPluginStrategy installStrategy, IPluginsActivator activator, PluginGlobalState pluginGlobalState)
        {
            _installStrategy = installStrategy;
            _activator = activator;
            _pluginGlobalState = pluginGlobalState;
        }

        public IInstallPluginStrategy GetInstallStrategy()
        {
            return _installStrategy;
        }

        public IEnumerable<IPlugin> GetPlugins()
        {
            _activator.LoadInstalledPlugins();
            return _pluginGlobalState.Plugins;
        }
    }
}
