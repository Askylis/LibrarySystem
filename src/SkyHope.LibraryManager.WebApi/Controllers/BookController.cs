using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private readonly DatabaseOptions _databaseOptions;
        public BookController(DatabaseOptions databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }
        [HttpGet]
        public ActionResult<IEnumerable<HttpModels.Book>> GetAll()
        {
            var results = new List<HttpModels.Book>();
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var availableBooks = context.Books.Where(b => b.IsAvailable && !b.IsDeleted).ToList();

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
        public ActionResult<HttpModels.Book> GetById(int id)
        {
            using (var context = new LibraryContext(_databaseOptions.ConnectionString))
            {
                var book = context.Books.FirstOrDefault(b => b.BookId == id);
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
        }

        [HttpPatch]
        public ActionResult<HttpModels.Book> Delete(HttpModels.Book book)
        {

        }

        [HttpPatch]
        public ActionResult<HttpModels.Book> CheckOut(HttpModels.Book book)
        {

        }
    }
}
