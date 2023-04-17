using System;

namespace PluginFramework.CustomPlugin.Zipping
{
    public class PluginFrameworkFilesFilter : IZipFileFilter
    {
        private const String PluginFrameworkName = "PluginFramework";
        private const String LSlicerName = "LSlicer";

        public bool Filter(string fileName)
        {
            return !fileName.Contains(PluginFrameworkName) 
                && !fileName.Contains(LSlicerName);
        }
    }
}
