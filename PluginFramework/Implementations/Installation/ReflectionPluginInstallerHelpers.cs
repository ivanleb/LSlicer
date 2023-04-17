using PluginFramework.Core.Configuration;
using PluginFramework.CustomPlugin.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

internal static class ReflectionPluginInstallerHelpers
{
    public static (List<PluginConfig> pluginConfigs, List<Assembly> assembliesWithPlugins) GetConfigsAndAssemblies(IEnumerable<Type> types)
    {
        var assembliesWithPlugins = new List<Assembly>();
        var pluginConfigs = new List<PluginConfig>();

        foreach (var pluginType in types)
        {
            var assemblyName = pluginType.Assembly.GetName();
            PluginConfig pluginConfig = new PluginConfig()
            {
                Name = pluginType.Name,
                AssemblyName = assemblyName.Name,
                Namespace = pluginType.Namespace,
                PluginName = ReflectionHelper.GeAssemblyName(assemblyName)
            };
            pluginConfigs.Add(pluginConfig);
            assembliesWithPlugins.Add(pluginType.Assembly);
        }

        return (pluginConfigs, assembliesWithPlugins);
    }
}