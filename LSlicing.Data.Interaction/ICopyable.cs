namespace LSlicer.Data.Interaction
{
    public interface ICopyable<T> where T : IIdentifier
    {
        T Copy(int newId);
    }
}
