using log4net;
using log4net.Config;

namespace GradientSpaceSliceEngine
{
    public static class Logger
    {
        private static ILog log = LogManager.GetLogger("GradientSpaceSupportEngineLogger");
        
        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
