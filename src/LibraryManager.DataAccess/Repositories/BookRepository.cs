using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class BookRepository : ILibraryManagerRepository<Book>
    {
        private readonly DatabaseOptions _databaseOptions;
        public BookRepository(IOptions<DatabaseOptions> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value;
        }
        public async Task<bool> TryAddAsync(Book entity)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                try
                {
                    await context.Books.AddAsync(entity)
                                        .ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<bool> TryDeleteAsync(int id)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                try
                {
                    var book = await context.Books.SingleOrDefaultAsync(b => b.BookId == id).ConfigureAwait(false);
                    if (book == null)
                    {
                        return false;
                    }
                    book.IsDeleted = true;
                    await context.SaveChangesAsync()
                        .ConfigureAwait(false);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Book?> GetEntityAsync(int id)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                return await context.Books.FirstOrDefaultAsync(b => b.BookId == id)
                    .ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(Book dbEntity, Book entity)
        {
            throw new NotImplementedException();
        }
    }
}
