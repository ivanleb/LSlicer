using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction
{
    public class ModelToSceneLoadingSpec
    {
        public ModelToSceneLoadingSpec(string pathToFile, int partId)
        {
            PathToFile = pathToFile;
            PartId = partId;
        }

        public String PathToFile { get; }
        public Int32 PartId { get; }
    }
}
