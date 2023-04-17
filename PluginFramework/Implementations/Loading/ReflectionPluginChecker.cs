using System;
using PluginFramework.Core.Loading;

namespace PluginFramework.Implementations.Loading
{
    public class ReflectionPluginChecker : IPluginChecker
    {
        public void CheckLoadedPluginCount(int pluginCount)
        {
            if (pluginCount > 1)
                throw new ApplicationException("Should be only one plugin");
            if (pluginCount < 1)
                throw new ApplicationException("No plugins in assembly");
        }
    }
}
