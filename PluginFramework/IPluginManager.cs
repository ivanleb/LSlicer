using PluginFramework.Installation;
using System.Collections.Generic;

namespace PluginFramework
{
    public interface IPluginManager
    {
        IEnumerable<IPlugin> GetPlugins();
        IInstallPluginStrategy GetInstallStrategy();
    }
}
