using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class AuthorRepository : ILibraryManagerRepository<Author>
    {
        private readonly DatabaseOptions _databaseOptions;
        public AuthorRepository(DatabaseOptions databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }
        public Task Add(Author entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Author>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Author?> GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Author dbEntity, Author entity)
        {
            throw new NotImplementedException();
        }
    }
}
