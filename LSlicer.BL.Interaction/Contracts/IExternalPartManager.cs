namespace LSlicer.BL.Interaction
{
    public interface IExternalPartManager
    {
        bool Append(ModelToSceneLoadingSpec spec);
        void Detach(int partId);
        void Copy(int oldPartId, int newPartId);
    }
}
