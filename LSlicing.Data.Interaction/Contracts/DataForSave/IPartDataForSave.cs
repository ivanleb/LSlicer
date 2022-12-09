namespace LSlicer.Data.Interaction
{
    public interface IPartDataForSave
    {
        IPartSpec Spec { get; }
        //IPartTransform Transform { get; }
        int LinkToParentPart { get; }
        IOperationInfo[] Operations { get; } 
    }
}
