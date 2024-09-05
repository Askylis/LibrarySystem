using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess.Repositories;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BookController : Controller
    {
        private const int maxBookCount = 3;
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
        public async Task<ActionResult<IEnumerable<HttpModels.Book>>> GetAllAsync()
        {
            var results = new List<HttpModels.Book>();
            var allBooks = await _bookRepository.GetAllAsync();
            var availableBooks = allBooks.Where(b => b.IsAvailable && !b.IsDeleted).ToList();

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
        public async Task<ActionResult<HttpModels.Book>> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetEntityAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var result = new HttpModels.Book
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

        [HttpPatch]
        public async Task<ActionResult> DeleteAsync(HttpModels.Book book)
        {
            if (! await _bookRepository.TryDeleteAsync(book.BookId))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult> CheckOutAsync(HttpModels.Book book, HttpModels.User user)
        {
            var bookToUpdate = await _bookRepository.GetEntityAsync(book.BookId);
            if (bookToUpdate is null)
            {
                return NotFound();
            }

            if (!bookToUpdate.IsAvailable)
            {
                return BadRequest("This book is not available for checkout.");
            }

            var userToAssign = await _userRepository.GetEntityAsync(user.UserId);
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

            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult> CheckInAsync(HttpModels.Book book)
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

            var bookToAdd = new Book
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                DewyClass = book.DewyClass,
                AuthorName = author.Name,
                Isbn = book.Isbn,
                Year = book.Year  
            };
            if (! await _bookRepository.TryAddAsync(bookToAdd))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

            return Created();
        }
    }
}