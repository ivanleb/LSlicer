using System;

namespace PluginFramework.Core
{
    public class PluginActionSpec
    {
        public PluginActionSpec(string resultPath, IProgress<string> progress)
        {
            ResultPath = resultPath;
            Progress = progress;
        }

        public string ResultPath { get; }
        public IProgress<string> Progress { get; }

        public static PluginActionSpec CreateEmpty() => new PluginActionSpec("", new Progress<string>());
    }
}
