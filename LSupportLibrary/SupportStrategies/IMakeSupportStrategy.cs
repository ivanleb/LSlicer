using LSlicer.Data.Interaction;

namespace LSupportLibrary
{
    public interface IMakeSupportStrategy
    {
        void Load(IPart part);
        void ApplyTransform(IPartTransform partTransform);
        void Prepare(IPart part);
        IPart[] GenerateSupports(IPart part, string supportPath);
        void WriteToFile(string filePath);
    }
}
