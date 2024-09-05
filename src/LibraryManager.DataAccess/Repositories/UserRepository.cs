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

        public async Task<bool> TryAddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TryDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetEntityAsync(int id)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                return await context.Users.FirstOrDefaultAsync(b => b.UserId == id)
                    .ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(User dbEntity, User entity)
        {
            throw new NotImplementedException();
        }
    }
}
