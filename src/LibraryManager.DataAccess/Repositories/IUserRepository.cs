using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager.DataAccess.Repositories
{
    public interface IUserRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetEntityAsync(int id);
        Task<bool> TryAddAsync(TEntity entity);
        Task UpdateAsync(TEntity dbEntity, TEntity entity);
        Task<bool> TryDeleteAsync(int id);
    }
}
