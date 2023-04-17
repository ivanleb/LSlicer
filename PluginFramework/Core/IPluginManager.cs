using System.Collections.Generic;
using PluginFramework.Core.Installation;

namespace PluginFramework.Core
{
    public interface IPluginManager
    {
        IEnumerable<IPlugin> GetPlugins();
        IInstallPluginStrategy GetInstallStrategy();
    }
}
