using JsonWrapper;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestEngineStub
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger.Launch();
            Logger.InitLogger();
            var interpret = new CmdLineInterpreter();

            var cmdLine = args.Aggregate((r, s) => r += (' ' + s));

            interpret.SetLine(cmdLine);
            JobSpecification spec = interpret.GetJobSpecification();
            bool result = JobSpecChecker.Check(spec);
            Console.WriteLine($"Job spec is correct: {result}");

            if (!result) 
            {
                Logger.Log.Info("Test was failed.");
                return; 
            }

            Console.WriteLine($"Job is {spec.JobType.ToString()}");
            Logger.Log.Info($"Job is {spec.JobType.ToString()}.");

            Console.WriteLine("Start job");

            int progressTick = 100;
            for (int i = 1; i <= 100; i++)
            {
                Thread.Sleep(progressTick);
                Console.WriteLine($"Progress: {i} %");
            }

            Console.WriteLine("End job");
            try
            {
                WriteResult(spec);
            }
            catch (Exception e)
            {
                Logger.Log.Error("Error: " + e.Message);
            }

            Logger.Log.Info("Test was passed.");
        }

        static void WriteResult(JobSpecification spec)
        {
            ISlicingInfo[] slicingInfos = new ISlicingInfo[spec.OutputFileInfo.Count];
            int i = 0;
            Console.WriteLine("Write files");
            foreach (var file in spec.OutputFileInfo)
            {
                var path = (spec.JobType == EJobType.MakeSupports) ?
                    Path.GetFileNameWithoutExtension(file.FullName) + "_s" + Path.GetExtension(file.FullName) :
                    file.FullName;

                using (var writer = new StreamWriter(path))
                {
                    writer.Write("test_file");
                    Console.WriteLine($"File {path} was written.");
                    Logger.Log.Info($"File {path} was written.");
                }
                slicingInfos[i] = new SlicingInfo() { Count = (i + 100), Thiсkness = 0.01, FilePath = path };
                i++;
            }
            Console.WriteLine($"Slicing info writing {spec.ResultFileInfo.FullName}.");
            Logger.Log.Info($"Slicing info writing {spec.ResultFileInfo.FullName}.");
            JsonSlicingResultProvider.WriteInfo(slicingInfos, spec.ResultFileInfo);
        }
    }
}
