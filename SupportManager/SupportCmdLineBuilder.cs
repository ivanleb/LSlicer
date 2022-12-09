using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportManager
{
    public class SupportCmdLineBuilder
    {
        private CmdLine _cmdLine = new CmdLine(new List<string>());

        public SupportCmdLineBuilder StartingProcess(string programFilePath)
        {
            if (_cmdLine.Args.Count > 0) throw new ArgumentException("Program path has already set");
            _cmdLine.Args.Add(programFilePath);
            return this;
        }

        public SupportCmdLineBuilder JobCommand(string jobCommand)
        {
            _cmdLine.Args.Add(jobCommand);
            return this;
        }

        public SupportCmdLineBuilder JobSpecification(string specificationFilePath)
        {
            _cmdLine.Args.Add("-j");
            _cmdLine.Args.Add(specificationFilePath);
            return this;
        }

        public SupportCmdLineBuilder SupportedParts(params string[] partFilePaths)
        {
            foreach (var filePath in partFilePaths)
            {
                _cmdLine.Args.Add("-l");
                _cmdLine.Args.Add(filePath);
            }
            return this;
        }

        public SupportCmdLineBuilder OutputPath(params string[] outputPath)
        {
            foreach (var filePath in outputPath)
            {
                _cmdLine.Args.Add("-o");
                _cmdLine.Args.Add(filePath);
            }
            return this;
        }

        public SupportCmdLineBuilder EngineResult(string filePath)
        {
            _cmdLine.Args.Add("-r");
            _cmdLine.Args.Add(filePath);
            return this;
        }

        public CmdLine Build() 
        {
            return _cmdLine;
        }
    }
}
