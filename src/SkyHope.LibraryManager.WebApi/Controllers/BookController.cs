using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Repositories;
using DataAccessBook = LibraryManager.DataAccess.Models.Book;
using HttpBook = SkyHope.LibraryManager.WebApi.HttpModels.Book;
using SkyHope.LibraryManager.WebApi.HttpModels.Request;
using SkyHope.LibraryManager.WebApi.HttpModels.Response;
using LibraryManager.DataAccess;
using Microsoft.Extensions.Options;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess.Specifications;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private readonly LibraryRepository _repository;
        private readonly LibraryOptions _libraryOptions;
        private readonly ILogger<BookController> _logger;
        public BookController(LibraryRepository repository,
            ILogger<BookController> logger,
            IOptions<LibraryOptions> libraryOptions)
        {
            _repository = repository;
            _libraryOptions = libraryOptions.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HttpBook>>> GetAllAsync()
        {
            var results = new List<HttpBook>();
            var allBooks = await _repository.ListAsync(new NotDeletedSpecification());

            foreach (var book in allBooks)
            {
                results.Add(new HttpBook
                {
                    Title = book.Title,
                    BookId = book.BookId,
                    AuthorId = book.AuthorId,
                    DewyClass = book.DewyClass,
                    Isbn = book.Isbn,
                    Year = book.Year
                });
            }
            _logger.LogInformation($"GetAllAsync in BooksController returned {allBooks.Count} available books.");
            return Ok(results);
        }

        [HttpGet]
        public async Task<ActionResult<HttpBook>> GetByIdAsync(int id)
        {
            var book = await _repository.FindAsync<Book>(id);
            if (book == null)
            {
                return NotFound();
            }

            var result = new HttpBook
            {
                Title = book.Title,
                BookId = book.BookId,
                AuthorId = book.AuthorId,
                DewyClass = book.DewyClass,
                Isbn = book.Isbn,
                Year = book.Year
            };

            return Ok(result);
        }

        [HttpDelete("{bookId}")]
        public async Task<ActionResult> DeleteAsync(int bookId)
        {
            var book = await _repository.FindAsync<Book>(bookId);

            if (book is null)
            {
                return NotFound();
            }

            _repository.Delete(book);
            await _repository.SaveAsync();

            return Ok();
        }

        [HttpPut("{bookId}")]
        public async Task<ActionResult<CheckoutResponse>> CheckOutAsync(int bookId, [FromBody]CheckoutRequest request)
        {
            var bookToUpdate = await _repository.FindAsync<Book>(bookId);
            if (bookToUpdate is null)
            {
                return NotFound();
            }

            if (!bookToUpdate.IsAvailable)
            {
                return new CheckoutResponse { ResponseType = CheckoutResponseType.Unavailable };
            }

            var userToAssign = await _userRepository.GetEntityAsync(request.UserId);
            if (userToAssign is null)
            {
                return BadRequest("Could not find user to assign to book.");
            }

            if (userToAssign.CheckedOutBooks.Count >= _libraryOptions.MaxBookCount)
            {
                return new CheckoutResponse { ResponseType= CheckoutResponseType.TooManyBooks };
            }

            if (userToAssign.LateFeeDue > 0)
            {
                return new CheckoutResponse { ResponseType = CheckoutResponseType.LateFeesOverdue };
            }

            bookToUpdate.IsAvailable = false;
            bookToUpdate.UserId = userToAssign.UserId;
            bookToUpdate.DueDate = DateTime.Now.AddDays(_libraryOptions.DueInDays);
            userToAssign.CheckedOutBooks.Add(bookToUpdate);

            return new CheckoutResponse { ResponseType = CheckoutResponseType.Success };
        }

        [HttpPut("{bookId}")]
        public async Task<ActionResult> CheckInAsync(int bookId)
        {
            var bookToUpdate = await _bookRepository.GetEntityAsync(bookId);
            if (bookToUpdate is null)
            {
                return NotFound();
            }

            bookToUpdate.IsAvailable = true;
            bookToUpdate.UserId = null;

            if (bookToUpdate.DueDate < DateTime.UtcNow)
            {
                var daysLate = DateTime.UtcNow - bookToUpdate.DueDate.Value;
                bookToUpdate.User.LateFeeDue = daysLate.Days * _libraryOptions.LateFeePerDay;
            }

            bookToUpdate.User.CheckedOutBooks.Remove(bookToUpdate);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> AddBookAsync(HttpBook book)
        {
            var author = await _authorRepository.GetEntityAsync(book.AuthorId);
            if (author is null)
            {
                return BadRequest("Author ID provided is not valid.");
            }

            var bookToAdd = new DataAccessBook
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                DewyClass = book.DewyClass,
                Isbn = book.Isbn,
                Year = book.Year
            };
            if (!await _bookRepository.TryAddAsync(bookToAdd))
            {
                _logger.LogError("An unexpected occurred while trying to add a new book with AddBookAsync().");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

            return Created();
        }

        [HttpGet]
        public async Task<ActionResult<HttpBook>> GetBooksByAuthorAsync(int authorId)
        {
            var results = new List<HttpBook>();
            var booksByAuthor = await _bookRepository.GetBooksByAuthorAsync(authorId);

            foreach (var book in booksByAuthor)
            {
                results.Add(new HttpBook
                {
                    Title = book.Title,
                    BookId = book.BookId,
                    AuthorId = book.AuthorId,
                    DewyClass = book.DewyClass,
                    Isbn = book.Isbn,
                    Year = book.Year
                });
            }

            return Ok(results);
        }

        [HttpGet]
        public async Task<ActionResult<List<HttpBook>>> GetBooksByDewyAsync(int dewyValue)
        {
            var results = new List<HttpBook>();
            var booksByDewy = await _bookRepository.GetBooksByDewyCodeAsync(dewyValue);

            foreach (var book in booksByDewy)
            {
                results.Add(new HttpBook
                {
                    Title = book.Title,
                    BookId = book.BookId,
                    AuthorId = book.AuthorId,
                    DewyClass = book.DewyClass,
                    Isbn = book.Isbn,
                    Year = book.Year
                });
            }

            return Ok(results);
        }

        [HttpGet("{startYear}/{endYear}")]
        public async Task<ActionResult<HttpBook>> GetBooksByYearAsync(int startYear, int endYear)
        {
            var results = new List<HttpBook>();
            var booksByYear = await _repository.ListAsync(new YearSpecification(startYear, endYear));

            foreach (var book in booksByYear)
            {
                results.Add(new HttpBook
                {
                    Title = book.Title,
                    BookId = book.BookId,
                    AuthorId = book.AuthorId,
                    DewyClass = book.DewyClass,
                    Isbn = book.Isbn,
                    Year = book.Year
                });
            }

            return Ok(results);
        }
    }
}