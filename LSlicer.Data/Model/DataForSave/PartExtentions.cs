using LSlicer.Data.Interaction;

namespace LSlicer.Data.Model
{
    public static class PartExtention
    {
        public static PartDataForSave GetChangesForSave(this IPart part, IOperationInfo[] operations) 
            => new PartDataForSave(part, operations);
        
        public static bool IsSupport(this IPart part) => part.PartSpec.PartType == LSlicer.Data.Interaction.PartType.Support;
        public static bool IsVolume(this IPart part) => part.PartSpec.PartType == LSlicer.Data.Interaction.PartType.Volume;

    }
}
