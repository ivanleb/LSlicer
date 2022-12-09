namespace LSlicer.BL.Interaction
{
    public interface ITaskSpec
    {
        int PartId { get; set; }
        string FilePath { get; set; }
        string ResultFilePath { get; set; }
    }
}
