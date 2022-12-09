using PluginFramework.Installation;
using System.Collections.Generic;

namespace PluginFramework
{
    public class PluginManager : IPluginManager
    {
        private readonly IInstallPluginStrategy _installStrategy;
        private readonly IPluginsActivator _activator;

        public PluginManager(IInstallPluginStrategy installStrategy, IPluginsActivator activator)
        {
            _installStrategy = installStrategy;
            _activator = activator;
        }

        public IInstallPluginStrategy GetInstallStrategy()
        {
            return _installStrategy;
        }

        public IEnumerable<IPlugin> GetPlugins()
        {
            _activator.LoadInstalledPlugins();
            return PluginGlobalState.Plugins;
        }
    }
}
