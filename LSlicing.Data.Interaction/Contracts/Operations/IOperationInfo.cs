namespace LSlicer.Data.Interaction
{
    public interface IOperationInfo
    {
        string Name { get; }
        OperationType Type { get; }
    }

    public enum OperationType
    {
        Loading,
        Transforming,
        Slicing,
        Supporting,
        Сopying
    }
}
