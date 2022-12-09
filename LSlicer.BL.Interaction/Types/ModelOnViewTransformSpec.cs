using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction.Types
{
    public class ModelOnViewTransformSpec
    {
        public ModelOnViewTransformSpec(int partId, IPartTransform transform)
        {
            PartId = partId;
            Transform = transform;
        }

        public int PartId { get; }
        public IPartTransform Transform { get; }
    }
}
