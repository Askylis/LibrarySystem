using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Repositories
{
    public interface IBookRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetEntityAsync(int id);
        Task<bool> TryAddAsync(TEntity entity);
        Task UpdateAsync(TEntity dbEntity, TEntity entity);
        Task<bool> TryDeleteAsync(int id);
        Task<List<Book>> GetBooksByAuthorAsync(int authorId);
        Task<List<Book>> GetBooksByDewyCodeAsync(int dewyCode);
        Task<List<Book>> GetBooksByYearAsync(int startYear, int endYear);
    }
}
