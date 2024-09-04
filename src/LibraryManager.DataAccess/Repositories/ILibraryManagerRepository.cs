namespace LibraryManager.DataAccess.Repositories
{
    public interface ILibraryManagerRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity?> GetEntity(int id);
        Task Add(TEntity entity);
        Task Update(TEntity dbEntity, TEntity entity);
        Task Delete(int id);
    }
}
