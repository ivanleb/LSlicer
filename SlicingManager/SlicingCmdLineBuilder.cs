using LSlicer.Helpers;
using System;
using System.Collections.Generic;

namespace SlicingManager
{
    public class SlicingCmdLineBuilder
    {
        private CmdLine _cmdLine = new CmdLine(new List<string>());

        public SlicingCmdLineBuilder StartingProcess(string programFilePath)
        {
            if (_cmdLine.Args.Count > 0) throw new ArgumentException("Program path has already set");
            _cmdLine.Args.Add(programFilePath);
            return this;
        }

        public SlicingCmdLineBuilder JobCommand(string jobCommand)
        {
            _cmdLine.Args.Add(jobCommand);
            return this;
        }

        public SlicingCmdLineBuilder JobSpecification(string specificationFilePath)
        {
            _cmdLine.Args.Add("-j");
            _cmdLine.Args.Add(specificationFilePath);
            return this;
        }

        public SlicingCmdLineBuilder SlicingParts(params string[] slicingPartFilePath)
        {
            foreach (var filePath in slicingPartFilePath)
            {
                _cmdLine.Args.Add("-l");
                _cmdLine.Args.Add(filePath);
            }
            return this;
        }

        public SlicingCmdLineBuilder OutputPath(params string[] outputPath)
        {
            foreach (var filePath in outputPath)
            {
                _cmdLine.Args.Add("-o");
                _cmdLine.Args.Add(filePath);
            }
            return this;
        }

        public SlicingCmdLineBuilder EngineResult(string filePath) 
        {
            _cmdLine.Args.Add("-r");
            _cmdLine.Args.Add(filePath);
            return this;
        }

        public SlicingCmdLineBuilder NetAddress(string netAddress)
        {
            _cmdLine.Args.Add("-a");
            _cmdLine.Args.Add(netAddress);
            return this;
        }

        public CmdLine Build()
        {
            return _cmdLine;
        }
    }
}
