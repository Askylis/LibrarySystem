using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Repositories;
using DataAccessBook = LibraryManager.DataAccess.Models.Book;
using HttpBook = SkyHope.LibraryManager.WebApi.HttpModels.Book;
using SkyHope.LibraryManager.WebApi.HttpModels.Request;
using SkyHope.LibraryManager.WebApi.HttpModels.Response;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private const int MaxBookCount = 3;
        private const int dueInDays = 14;
        private const decimal lateFeePerDay = 0.5m;
        private readonly BookRepository _bookRepository;
        private readonly UserRepository _userRepository;
        private readonly AuthorRepository _authorRepository;
        private readonly ILogger<BookController> _logger;
        public BookController(BookRepository bookRepository, UserRepository userRepository, AuthorRepository authorRepository, ILogger<BookController> logger)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _authorRepository = authorRepository;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into BookController");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HttpBook>>> GetAllAsync()
        {
            var results = new List<HttpBook>();
            var allBooks = await _bookRepository.GetAllAsync();
            var availableBooks = allBooks.Where(b => b.IsAvailable && !b.IsDeleted).ToList();

            if (!availableBooks.Any())
            {
                return NotFound();
            }

            foreach (var book in availableBooks)
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
            _logger.LogInformation($"GetAllAsync in BooksController returned {availableBooks.Count} available books.");
            return Ok(results);
        }

        [HttpGet]
        public async Task<ActionResult<HttpBook>> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetEntityAsync(id);
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
            // TODO: Check it exists and return 404 if it deosn't

            if (!await _bookRepository.TryDeleteAsync(bookId))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

            return Ok();
        }

        [HttpPut("{bookId}")]
        public async Task<ActionResult<CheckoutResponse>> CheckOutAsync(int bookId, [FromBody]CheckoutRequest request)
        {
            var bookToUpdate = await _bookRepository.GetEntityAsync(bookId);
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

            if (userToAssign.CheckedOutBooks.Count >= maxBookCount)
            {
                return new CheckoutResponse { ResponseType= CheckoutResponseType.TooManyBooks };
            }

            if (userToAssign.LateFeeDue > 0)
            {
                return new CheckoutResponse { ResponseType = CheckoutResponseType.UserIsOLateFeesOverdue };
            }

            bookToUpdate.IsAvailable = false;
            bookToUpdate.UserId = userToAssign.UserId;
            bookToUpdate.User = userToAssign;
            bookToUpdate.DueDate = DateTime.Now.AddDays(dueInDays);
            userToAssign.CheckedOutBooks.Add(bookToUpdate);

            return new CheckoutResponse { ResponseType = CheckoutResponseType.Success };
        }

        [HttpPut]
        public async Task<ActionResult> CheckInAsync(HttpBook book)
        {
            var bookToUpdate = await _bookRepository.GetEntityAsync(book.BookId);
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

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> AddBookAsync(HttpModels.Book book)
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

            return Created();
        }

        [HttpGet]
        public async Task<ActionResult<HttpBook>> GetBooksByAuthorAsync(int authorId)
        {
            var results = new List<HttpBook>();
            var allBooks = await _bookRepository.GetAllAsync();
            var booksByAuthor = allBooks.Where(b => b.AuthorId == authorId).ToList();
            if (booksByAuthor.Count == 0)
            {
                return BadRequest("Unable to find any books by the specified author.");
            }

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
        public async Task<ActionResult<HttpBook>> GetBooksByDewyAsync(int dewyValue)
        {
            var results = new List<HttpBook>();
            var allBooks = await _bookRepository.GetAllAsync();
            var booksByDewy = allBooks.Where(b => b.DewyClass == dewyValue).ToList();
            if (booksByDewy.Count == 0)
            {
                return BadRequest("Unable to find any books in this Dewy class.");
            }

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

        [HttpGet]
        public async Task<ActionResult<HttpBook>> GetBooksByYearAsync(int startYear, int endYear)
        {
            var results = new List<HttpBook>();
            var allBooks = await _bookRepository.GetAllAsync();
            var booksByYear = allBooks.Where(b => b.Year < endYear && b.Year > startYear).ToList();
            if (booksByYear.Count == 0)
            {
                return BadRequest("Unable to find any books within this time period.");
            }

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