using LSlicer.BL.Domain;
using LSlicer.BL.Interaction;
using LSlicer.Helpers;
using System.Linq;

namespace SupportManager
{
    public class SupportEngineInvoker<T> : EngineInvokerBase<T>
    {
        public SupportEngineInvoker(ILoggerService loggerService): base(loggerService)
        {
        }

        protected override CmdLine InterpretateSpec(IEngineTask engineTask) 
        {
            var builder = new SupportCmdLineBuilder();
            return builder
                .StartingProcess(engineTask.Engine.FullName)
                .JobCommand(engineTask.JobType.ToString())
                .JobSpecification(engineTask.JobSpec.FullName)
                .SupportedParts(engineTask.PartSpec.Select(x => x.FilePath).ToArray())
                .OutputPath(engineTask.PartSpec.Select(x => x.ResultFilePath).ToArray())
                .EngineResult(engineTask.JobResult.FullName)
                .Build();
        }
    }
}
