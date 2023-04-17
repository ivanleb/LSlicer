using System;

namespace PluginFramework.Logging
{
    public class DebugPluginLogger : ILogger
    {
        public void Error(string message)
        {
            System.Diagnostics.Debugger.Log(0, "Error", message);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debugger.Log(0, "Info", message);
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debugger.Log(0, "Warning", message);
        }
    }
}
