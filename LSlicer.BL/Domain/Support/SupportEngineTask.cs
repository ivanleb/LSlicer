using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using System.IO;

namespace LSlicer.BL.Domain
{
    public class SupportEngineTask : IEngineTask
    {
        public SupportEngineTask(ITaskSpec[] partSpecs, FileInfo engine, FileInfo jobSpec, FileInfo jobResult, int numberFrom = 0)
        {
            PartSpec = partSpecs;
            Engine = engine;
            JobSpec = jobSpec;
            JobResult = jobResult;

            foreach (var spec in PartSpec)
            {
                var newFilePath = Path.Combine(Path.GetDirectoryName(spec.FilePath),
                    Path.GetFileNameWithoutExtension(spec.FilePath) + "_s" + Path.GetExtension(spec.FilePath));
                IPartSpec supportPartSpec = new PartSpec(numberFrom, newFilePath);
                //spec.Support = new Support(supportPartSpec, spec.PartId);
            }
        }

        public ITaskSpec[] PartSpec { get; }

        public FileInfo Engine { get; }

        public FileInfo JobSpec { get; }

        public EJobType JobType => EJobType.MakeSupports;

        public FileInfo JobResult { get; }
    }
}
