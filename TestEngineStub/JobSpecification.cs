using System.Collections.Generic;
using System.IO;

namespace TestEngineStub
{
    public class JobSpecification
    {
        public EJobType? JobType { get; set; }
        public IList<FileInfo> ModelFileInfo { get; set; } = new List<FileInfo>();
        public IList<FileInfo> OutputFileInfo { get; set; } = new List<FileInfo>();
        public FileInfo ParametersFileInfo { get; set; }
        public FileInfo ResultFileInfo { get; set; }
    }
}
