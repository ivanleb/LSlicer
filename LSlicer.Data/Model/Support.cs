using LSlicer.Data.Interaction;
using LSlicing.Data.Interaction;

namespace LSlicer.Data.Model
{
    public class Support : Part
    {
        public Support(IPartSpec partSpec, int link) : base(partSpec)
        {
            LinkToParentPart = link;
            partSpec.PartType = PartType.Support;
        }
    }
}
