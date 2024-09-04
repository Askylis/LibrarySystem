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
        public async Task TryAddAsync(Author entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TryDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Author?> GetEntityAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Author dbEntity, Author entity)
        {
            throw new NotImplementedException();
        }
    }
}
