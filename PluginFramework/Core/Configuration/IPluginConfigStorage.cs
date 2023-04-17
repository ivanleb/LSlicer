using System.Collections.Generic;
using System.IO;

namespace PluginFramework.Core.Configuration
{
    public interface IPluginConfigStorage
    {
        PluginConfig[] GetInstalledPluginsFromDescription();
        void AddInstalledPluginToConfigStorage(IEnumerable<PluginConfig> pluginConfigs);
        void CreatePluginConfig(IEnumerable<PluginConfig> pluginConfigs, DirectoryInfo resultDir);
        void RemovePluginFromDescriptions(IPlugin plugin);
        PluginConfig[] ReadPluginConfigs(DirectoryInfo pluginDirectoryInfo);
    }
}
