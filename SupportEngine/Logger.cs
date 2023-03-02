using log4net;
using log4net.Config;
using log4net.Repository;
using LSlicer.BL.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SupportEngine
{
    public static class Logger
    {
        private static ILog log = LogManager.GetLogger(Assembly.GetCallingAssembly(), "SupportEngineLogger");
        
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
