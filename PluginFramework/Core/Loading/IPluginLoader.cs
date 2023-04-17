using PluginFramework.Core.Configuration;
using System.IO;

namespace PluginFramework.Core.Loading
{
    public interface IPluginLoader
    {
        PluginConfig[] LoadPluginFromDirectory(DirectoryInfo pluginDirInfo);
    }
}
