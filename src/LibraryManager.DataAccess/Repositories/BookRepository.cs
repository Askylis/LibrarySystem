using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class BookRepository : ILibraryManagerRepository<Book>
    {
        public void Add(Book entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> GetAll()
        {
            throw new NotImplementedException();
        }

        public Book GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Book dbEntity, Book entity)
        {
            throw new NotImplementedException();
        }
    }
}
