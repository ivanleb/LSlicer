namespace PluginFramework
{
    public interface IInstallPluginStrategy
    {
        void Install(string pluginPackagePath);
        void MakePluginPackage(string pluginDirectoryPath, string targetDirectoryPath);
        void Uninstall(IPlugin plugin);
    }
}
