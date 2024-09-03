namespace LibraryManager.DataAccess.Repositories
{
    public interface ILibraryManagerRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetEntity(int id);
        void Add(TEntity entity);
        void Update(TEntity dbEntity, TEntity entity);
        void Delete(int id);
    }
}
