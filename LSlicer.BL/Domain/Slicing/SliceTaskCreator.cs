using LaserAprSlicer.BL.Interaction.Contracts;
using LaserAprSlicer.Data.Interaction;
using System.Linq;

namespace LaserAprSlicer.BL.Domain
{
    public class SliceTaskCreator
    {
        IEngineTask Create(IPart[] parts)
        {
            IPartSpec[] partSpecs = parts.Select(x => x.PartSpec).ToArray();
            return new SliceEngineTask(partSpecs);
        }
    }
}
