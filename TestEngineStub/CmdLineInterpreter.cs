using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
namespace TestEngineStub
{
    public class CmdLineInterpreter
    {
        private string _cmdLine;
        private JobSpecification _jobSpecification;

        public CmdLineInterpreter()
        {
            _jobSpecification = new JobSpecification();
        }

        public void SetLine(string cmd) 
        {
            Logger.Log.Info($"Input command line: \"{cmd}\"");
            _cmdLine = cmd;
            MakeSpec();
        }

        private void MakeSpec() 
        {
            _jobSpecification.ModelFileInfo = new List<FileInfo>();
            var cmdList = _cmdLine.Split(' ').ToList();
            _jobSpecification.JobType = (EJobType)Enum.Parse(typeof(EJobType), cmdList[0], true);
                       
            for (int i = 0; i < cmdList.Count; i++)
            {
                if (cmdList[i] == "-j")
                    _jobSpecification.ParametersFileInfo = new FileInfo(cmdList[i + 1]);

                if (cmdList[i] == "-l")
                    _jobSpecification.ModelFileInfo.Add(new FileInfo(cmdList[i + 1]));

                if (cmdList[i] == "-o")
                    _jobSpecification.OutputFileInfo.Add(new FileInfo(cmdList[i + 1]));

                if (cmdList[i] == "-r")
                    _jobSpecification.ResultFileInfo = new FileInfo(cmdList[i + 1]);
            }
        }

        public JobSpecification GetJobSpecification() => _jobSpecification;
    }
}
