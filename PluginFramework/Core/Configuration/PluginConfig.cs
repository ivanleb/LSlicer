using PluginFramework.Core;
using PluginFramework.CustomPlugin.Helpers;
using System;
using System.Reflection;

namespace PluginFramework.Core.Configuration
{
    [Serializable]
    public class PluginConfig
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public string PluginName { get; set; }
    }

    public static class PluginConfigExt
    {
        public static PluginConfig CreateConfig(this IPlugin plugin)
        {
            Type pluginType = plugin.GetType();
            AssemblyName assemblyName = pluginType.Assembly.GetName();
            return new PluginConfig
            {
                Name = pluginType.Name,
                AssemblyName = assemblyName.Name,
                Namespace = pluginType.Namespace,
                PluginName = ReflectionHelper.GeAssemblyName(assemblyName)
            };
        }

        public static bool TheSame(this PluginConfig cfg1, PluginConfig cfg2)
        {
            return cfg1.Name == cfg2.Name
                && cfg1.AssemblyName == cfg2.AssemblyName
                && cfg1.Namespace == cfg2.Namespace
                && cfg1.PluginName == cfg2.PluginName;
        }
    }
}
