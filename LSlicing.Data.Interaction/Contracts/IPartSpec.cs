using LSlicer.Data.Interaction;

namespace LSlicer.Data.Interaction
{
    public interface IPartSpec : IIdentifier
    {
        string MeshFilePath { get; set; }
        PartType PartType { get; set; }
    }
}