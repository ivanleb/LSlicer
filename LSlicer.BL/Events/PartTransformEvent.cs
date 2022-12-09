using LSlicer.Data.Interaction;
using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Events
{
    public class PartTransformEventParameter : IFrontOperationEventParameter
    {
        public PartTransformEventParameter(int partId, IPartTransform transform)
        {
            PartId = partId;
            Transform = transform;
        }

        public int PartId { get; }
        public IPartTransform Transform { get; }
    }

    public class PartLoadEventParameter : IFrontOperationEventParameter
    {
        public PartLoadEventParameter(IPartDataForSave partDataForSave)
        {
            PartDataForSave = partDataForSave;
        }

        public IPartDataForSave PartDataForSave { get; }
    }
}
