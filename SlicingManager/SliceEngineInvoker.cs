using LSlicer.BL.Domain;
using LSlicer.BL.Interaction;
using LSlicer.Helpers;
using System.Linq;

namespace SlicingManager
{
    public class SliceEngineInvoker<T> : EngineInvokerBase<T>
    {
        public SliceEngineInvoker(ILoggerService loggerService) :base(loggerService)
        {
        }

        protected override CmdLine InterpretateSpec(IEngineTask engineTask)
        {
            var builder = new SlicingCmdLineBuilder();
            return builder
                .StartingProcess(engineTask.Engine.FullName)
                .JobCommand(engineTask.JobType.ToString())
                .JobSpecification(engineTask.JobSpec.FullName)
                .SlicingParts(engineTask.PartSpec.Select(x => x.FilePath).ToArray())
                .OutputPath(engineTask.PartSpec.Select(x => x.ResultFilePath).ToArray())
                .EngineResult(engineTask.JobResult.FullName)
                //.NetAddress("tcp://127.0.0.1:4502/")
                .Build();
        }
    }
}
