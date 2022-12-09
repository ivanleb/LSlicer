using System.IO;

namespace LSlicer.BL.Interaction
{
    public interface IEngineTask
    {
        EJobType JobType { get; }
        ITaskSpec[] PartSpec { get; }
        FileInfo Engine { get; }
        FileInfo JobSpec { get; }
        FileInfo JobResult { get; }
    }
}
