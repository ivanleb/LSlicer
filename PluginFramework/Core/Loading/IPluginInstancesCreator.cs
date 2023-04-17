using System;
using System.Collections.Generic;
using System.Reflection;

namespace PluginFramework.Core.Loading
{
    public interface IPluginInstancesCreator<T>
    {
        IReadOnlyList<T> CreateInstances(Assembly assembly, Type pluginType);
    }
}
