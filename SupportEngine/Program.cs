using EngineHelpers;
using JsonWrapper;
using LSlicer.Data.Interaction;
using System;
using System.Linq;

namespace SupportEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            //Debugger.Launch();
            Logger.InitLogger();
            var interpret = new CmdLineInterpreter();

            var cmdLine = args.Aggregate((r, s) => r += (' ' + s));


            Logger.Log.Info($"Input command line: \"{cmdLine}\"");
            interpret.SetLine(cmdLine);
            JobSpecification spec = interpret.GetJobSpecification();
            bool result = SupportJobSpecChecker.Check(spec);
            Console.WriteLine($"Support job spec is correct: {result}");

            if (!result)
            {
                Logger.Log.Info("Test was failed.");
                return;
            }

            Console.WriteLine($"Job is {spec.JobType.ToString()}");
            Logger.Log.Info($"Job is {spec.JobType.ToString()}.");

            Console.WriteLine("Start job");

            ISupportInfo[] infos = SupportGenerator.Do(spec); 

            Console.WriteLine("End job");

            try
            {
                WriteResult(infos, spec);
            }
            catch (Exception e)
            {
                Logger.Log.Error("Error: " + e.Message);
            }

            Logger.Log.Info("Test was passed.");
        }

        static void WriteResult(ISupportInfo[] supportInfos, JobSpecification spec)
        {
            var logInfo = $"Support info was written  to {spec.ResultFileInfo.FullName}.";
            Console.WriteLine(logInfo);
            Logger.Log.Info(logInfo);
            JsonSupportResultProvider.WriteInfo(supportInfos, spec.ResultFileInfo);
        }
    }
}
