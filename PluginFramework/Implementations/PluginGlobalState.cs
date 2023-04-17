using System.Collections.Generic;
using PluginFramework.Core;
using PluginFramework.CustomPlugin.Helpers;
using PluginFramework.Implementations.Loading;

namespace PluginFramework.Implementations
{
    public class PluginGlobalState
    {
        private object _locker = new object();
        private List<IPlugin> _plugins = new List<IPlugin>();

        public PluginGlobalState(LoadContextContainer assemblyContainer)
        {
            AssemblyContainer = assemblyContainer;
        }

        public LoadContextContainer AssemblyContainer { get; }

        public IEnumerable<IPlugin> Plugins => _plugins;

        public void AddPlugin(IPlugin plugin)
        {
            lock (_locker)
                _plugins.Add(plugin);
        }

        public void RemovePlugin(IPlugin plugin)
        {
            lock (_locker)
            {
                string domainName = ReflectionHelper.GeAssemblyName(plugin.GetType().Assembly.GetName());
                AssemblyContainer.UnloadAssembly(domainName);
                _plugins.Remove(plugin);
            }
        }
    }
}
