using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class UserRepository : ILibraryManagerRepository<User>
    {
        public void Add(User entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(User dbEntity, User entity)
        {
            throw new NotImplementedException();
        }
    }
}
