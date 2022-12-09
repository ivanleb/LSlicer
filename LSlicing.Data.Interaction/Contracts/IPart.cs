using LSlicing.Data.Interaction;
using LSlicing.Data.Interaction.Contracts;
using System.Collections.Generic;

namespace LSlicer.Data.Interaction
{
    public interface IPart : IIdentifier, ICopyable<IPart>
    {
        IPartSpec PartSpec { get; }
        IPartTransform ResultTransform { get; }
        int LinkToParentPart { get; set; }
        void Transform(IPartTransform transform);      
    }
}
