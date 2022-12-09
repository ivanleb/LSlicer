namespace LSlicer.Data.Interaction
{
    //информация о поддержках для каждого файла поддержек появляющеяся после построения поддержек
    public interface ISupportInfo : IOperationInfo
    {
        string MeshFilePath { get; set; }
        string SupportFilePath { get; set; }
        string EasySliceSupportStructure { get; set; }
        string SupportParameters { get; set; }
    }
}
