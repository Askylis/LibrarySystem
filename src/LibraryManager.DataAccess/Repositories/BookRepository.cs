using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class BookRepository : ILibraryManagerRepository<Book>
    {
        private readonly DatabaseOptions _databaseOptions;
        public BookRepository(DatabaseOptions databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }
        public async Task Add(Book entity)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Book?> GetEntity(int id)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var book = await context.Books.FirstOrDefaultAsync(b => b.BookId == id);
                if (book == null)
                {
                    return null;
                }
                return book;
            }
        }

        public async Task Update(Book dbEntity, Book entity)
        {
            throw new NotImplementedException();
        }
    }
}
