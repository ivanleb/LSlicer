using LSlicer.BL.Interaction;
using System.IO;

namespace LSlicer.BL.Domain
{
    public class SliceEngineTask : IEngineTask
    {
        public SliceEngineTask(ITaskSpec[] partSpec, FileInfo enginePath, FileInfo parameters, FileInfo jobResult)
        {
            PartSpec = partSpec;
            Engine = enginePath;
            JobSpec = parameters;
            JobResult = jobResult;
        }

        public ITaskSpec[] PartSpec { get; }

        public FileInfo Engine { get; }

        public FileInfo JobSpec { get; }

        public EJobType JobType => EJobType.Slice;

        public FileInfo JobResult { get; }
    }
}
