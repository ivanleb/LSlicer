using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Data.Model
{
    public class SupportInfo : ISupportInfo
    {
        public string MeshFilePath { get; set; }
        public string SupportFilePath { get; set; }
        public string Name { get; set; }
        public string EasySliceSupportStructure { get; set; }
        public string SupportParameters { get; set; }
        public OperationType Type => OperationType.Supporting;
    }
}
