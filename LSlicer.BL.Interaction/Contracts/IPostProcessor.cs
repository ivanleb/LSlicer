namespace LSlicer.BL.Interaction
{
    public interface IPostProcessor<T>
    {
        void HandleResult(T[] infos);
    }
}
