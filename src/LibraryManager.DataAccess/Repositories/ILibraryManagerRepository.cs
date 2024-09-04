namespace LibraryManager.DataAccess.Repositories
{
    public interface ILibraryManagerRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetEntityAsync(int id);
        Task<bool> TryAddAsync(TEntity entity);
        Task UpdateAsync(TEntity dbEntity, TEntity entity);
        Task<bool> TryDeleteAsync(int id);
    }
}
