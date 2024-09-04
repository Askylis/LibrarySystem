using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private readonly DatabaseOptions _databaseOptions;
        private const int maxBookCount = 3;
        private const int dueInDays = 14;
        private const decimal lateFeePerDay = 0.5m;
        public BookController(DatabaseOptions databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HttpModels.Book>>> GetAllAsync()
        {
            var results = new List<HttpModels.Book>();
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var availableBooks = await context.Books.Where(b => b.IsAvailable && !b.IsDeleted).ToListAsync();

                if (!availableBooks.Any())
                {
                    return NotFound();
                }

                foreach (var book in availableBooks)
                {
                    results.Add(new HttpModels.Book
                    {
                        Title = book.Title,
                        BookId = book.BookId,
                        Author = book.AuthorName,
                        DewyClass = book.DewyClass,
                        Isbn = book.Isbn,
                        Year = book.Year
                    });
                }
                return Ok(results);
            }
        }

        [HttpGet]
        public async Task<ActionResult<HttpModels.Book>> GetByIdAsync(int id)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var book = await context.Books.FirstOrDefaultAsync(b => b.BookId == id);
                if (book is null)
                {
                    return NotFound();
                }

                var result = new HttpModels.Book
                {
                    Title = book.Title,
                    BookId = book.BookId,
                    Author = book.AuthorName,
                    DewyClass = book.DewyClass,
                    Isbn = book.Isbn,
                    Year = book.Year
                };

                return Ok(result);
            }
        }

        [HttpPatch]
        public async Task<ActionResult> DeleteAsync(HttpModels.Book book)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var result = await context.Books.FirstOrDefaultAsync(b => b.BookId == book.BookId);

                if (result is null)
                {
                    return NotFound();
                }

                result.IsDeleted = true;
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult> CheckOutAsync(HttpModels.Book book, HttpModels.User user)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var bookToUpdate = await context.Books.FirstOrDefaultAsync(b => b.BookId == book.BookId);
                if (bookToUpdate is null)
                {
                    return NotFound();
                }

                if (!bookToUpdate.IsAvailable)
                {
                    return BadRequest("This book is not available for checkout.");
                }

                var userToAssign = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (userToAssign is null)
                {
                    return NotFound("Could not find user to assign to book.");
                }

                if (userToAssign.CheckedOutBooks.Count >= maxBookCount)
                {
                    return BadRequest($"This user has {maxBookCount} books checked out, and may not check out more.");
                }

                if (userToAssign.LateFeeDue > 0)
                {
                    return BadRequest($"This user has ${userToAssign.LateFeeDue} in late fees due and cannot check out another book until this is paid off.");
                }

                bookToUpdate.IsAvailable = false;
                bookToUpdate.UserId = userToAssign.UserId;
                bookToUpdate.User = userToAssign;
                bookToUpdate.DueDate = DateTime.Now.AddDays(dueInDays);
                userToAssign.CheckedOutBooks.Add(bookToUpdate);
                await context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult> CheckInAsync(HttpModels.Book book)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var bookToUpdate = await context.Books.FirstOrDefaultAsync(b => b.BookId == book.BookId);
                if (bookToUpdate is null)
                {
                    return NotFound();
                }

                bookToUpdate.IsAvailable = true;
                bookToUpdate.User = null;
                bookToUpdate.UserId = null;

                if (bookToUpdate.DueDate < DateTime.UtcNow)
                {
                    var daysLate = DateTime.UtcNow - bookToUpdate.DueDate.Value;
                    bookToUpdate.User.LateFeeDue = daysLate.Days * lateFeePerDay;
                }

                bookToUpdate.User.CheckedOutBooks.Remove(bookToUpdate);
                await context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}