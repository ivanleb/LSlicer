namespace LSlicer.Data.Interaction
{
    //информация о нарезке для каждой детали появляющеяся после нарезания
    public interface ISlicingInfo : IOperationInfo
    {
        int Count { get; set; }
        double Thiсkness { get; set; }
        string FilePath { get; set; }
        int PartId { get; set; }
    }
}
