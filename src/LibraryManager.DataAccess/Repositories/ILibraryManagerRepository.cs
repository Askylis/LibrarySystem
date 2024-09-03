using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
