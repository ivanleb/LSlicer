namespace PluginFramework.Core
{
    public enum LoadType
    {
        Manual,
        Auto
    }

    public interface IPlugin
    {
        string Name { get; }
        LoadType LoadType { get; }
        void DoAction(PluginActionSpec spec);
    }
}
