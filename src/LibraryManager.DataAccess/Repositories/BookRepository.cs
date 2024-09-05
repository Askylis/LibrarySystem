using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class BookRepository : IBookRepository<Book>
    {
        private readonly DatabaseOptions _databaseOptions;
        public BookRepository(DatabaseOptions databaseOptions)
        {
            _databaseOptions = databaseOptions;
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
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                return await context.Books.Where(b => b.IsAvailable && !b.IsDeleted).ToListAsync();
            }
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

        public async Task<List<Book>> GetBooksByAuthorAsync(int authorId)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                return await context.Books.Where(b => b.AuthorId == authorId).ToListAsync();
            }
        }

        public async Task<List<Book>> GetBooksByDewyCodeAsync(int dewyCode)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                return await context.Books.Where(b => b.DewyClass == dewyCode).ToListAsync();
            }
        }

        public async Task<List<Book>> GetBooksByYearAsync(int startYear, int endYear)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                return await context.Books.Where(b => b.Year >= startYear && b.Year <= endYear).ToListAsync();
            }
        }
    }
}
