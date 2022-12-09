using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Data.Model
{
    public class LoadInfo : IOperationInfo
    {
        public LoadInfo(string name, string meshFile)
        {
            Name = name;
            MeshFile = meshFile;
        }

        public string Name { get; set; } = "Load part";
        
        public string MeshFile { get; }

        public OperationType Type => OperationType.Loading;
    }
}
