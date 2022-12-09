namespace LSlicer.BL.Interaction
{
    public interface IRepository<in T>
    {
        void Add(T entity);
        void Remove(T entity);
        void Update(T entity);
        void Save();
    }
}
