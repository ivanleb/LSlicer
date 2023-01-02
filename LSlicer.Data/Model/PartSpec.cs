using LSlicer.Data.Interaction;
using LSlicer.Data.Interaction;

namespace LSlicer.Data.Model
{
    public class PartSpec : IPartSpec
    {
        private int _partId;

        public PartSpec(int partId, string meshFilePath)
        {
            _partId = partId;
            MeshFilePath = meshFilePath;
        }

        public PartSpec(int partId, string meshFilePath, PartType type)
            : this(partId, meshFilePath)
        {
            PartType = type;
        }

        public PartSpec(int newId, IPartSpec oldSpec)
        {
            _partId = newId;
            MeshFilePath = oldSpec.MeshFilePath;
        }

        public PartSpec(){}

        public string MeshFilePath { get; set; }

        public int Id { get => _partId; set => _partId = value; }
        public PartType PartType { get ; set ; }
    }
}
