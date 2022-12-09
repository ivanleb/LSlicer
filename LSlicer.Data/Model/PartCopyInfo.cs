using LSlicer.Data.Interaction;
using System;

namespace LSlicer.Data.Model
{
    public class PartCopyInfo : IPartCopy
    {
        public int _copyPartId;

        public PartCopyInfo(string name, int copyPartId)
        {
            Name = name;
            _copyPartId = copyPartId;
        }

        public int CopyPartId => _copyPartId;

        public string Name { get; set; }

        public OperationType Type => OperationType.Сopying;
    }
}
