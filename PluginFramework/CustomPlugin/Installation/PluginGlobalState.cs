using System.Collections.Generic;
using PluginFramework.CustomPlugin.Helpers;

namespace PluginFramework.CustomPlugin.Installation
{
    public class PluginGlobalState
    {
        private static object _locker = new object();
        private static List<IPlugin> _plugins = new List<IPlugin>();

        public static AppDomainContainer DomainContainer = new AppDomainContainer();

        public static IEnumerable<IPlugin> Plugins => _plugins;

        public static void AddPlugin(IPlugin plugin)
        {
            lock (_locker)
                _plugins.Add(plugin);
        }

        public static void RemovePlugin(IPlugin plugin)
        {
            lock (_locker)
            {
                string domainName = ReflectionHelper.GetDomainName(plugin.GetType().Assembly.GetName());
                DomainContainer.UnloadDomain(domainName);
                _plugins.Remove(plugin);
            }
        }
    }
}
