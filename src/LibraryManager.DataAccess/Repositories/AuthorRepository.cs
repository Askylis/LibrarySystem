using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class AuthorRepository : ILibraryManagerRepository<Author>
    {
        public void Add(Author entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Author> GetAll()
        {
            throw new NotImplementedException();
        }

        public Author GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Author dbEntity, Author entity)
        {
            throw new NotImplementedException();
        }
    }
}
