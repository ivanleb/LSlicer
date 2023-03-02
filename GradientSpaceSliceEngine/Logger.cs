using log4net;
using log4net.Config;
using System.Reflection;

namespace GradientSpaceSliceEngine
{
    public static class Logger
    {
        private static ILog log = LogManager.GetLogger(Assembly.GetCallingAssembly(), "GradientSpaceSupportEngineLogger");
        
        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
        }
    }
}
