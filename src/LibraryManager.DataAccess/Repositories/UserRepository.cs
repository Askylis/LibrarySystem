using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class UserRepository : ILibraryManagerRepository<User>
    {
        private readonly DatabaseOptions _databaseOptions;
        public UserRepository(DatabaseOptions databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }

        public Task Add(User entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(User dbEntity, User entity)
        {
            throw new NotImplementedException();
        }
    }
}
