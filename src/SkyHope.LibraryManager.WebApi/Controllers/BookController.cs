﻿using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Repositories;
using DataAccessBook = LibraryManager.DataAccess.Models.Book;
using HttpBook = SkyHope.LibraryManager.WebApi.HttpModels.Book;
using SkyHope.LibraryManager.WebApi.HttpModels.Request;
using SkyHope.LibraryManager.WebApi.HttpModels.Response;
using LibraryManager.DataAccess;
using Microsoft.Extensions.Options;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess.Specifications.Books;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet("all")]
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

        [HttpGet("{bookId}")]
        public async Task<ActionResult<HttpBook>> GetByIdAsync(int bookId)
        {
            var book = await _repository.FindAsync<Book>(bookId);
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
            try
            {
                await _repository.SaveAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting book {book.Title}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpPut("checkout/{bookId}")]
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

            var userToAssign = await _repository.FindAsync<User>(bookId);
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

            try
            {
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking out book.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return new CheckoutResponse { ResponseType = CheckoutResponseType.Success };
        }

        [HttpPut("checkin/{bookId}")]
        public async Task<ActionResult> CheckInAsync(int bookId)
        {
            var bookToUpdate = await _repository.FindAsync<Book>(bookId);
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
            try
            {
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking in book.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> AddBookAsync(HttpBook book)
        {
            var author = await _repository.FindAsync<Author>(book.AuthorId);
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
            try
            {
                await _repository.AddAsync<Book>(bookToAdd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong adding book to the database.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _repository.SaveAsync();
            return Created();
        }

        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<HttpBook>> GetBooksByAuthorAsync(int authorId)
        {
            var results = new List<HttpBook>();
            var booksByAuthor = await _repository.ListAsync<Book>(new AuthorSpecification(authorId));

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

        [HttpGet("dewy/{dewyValue}")]
        public async Task<ActionResult<List<HttpBook>>> GetBooksByDewyAsync(int dewyValue)
        {
            var results = new List<HttpBook>();
            var booksByDewy = await _repository.ListAsync<Book>(new DewySpecification(dewyValue));

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
        public async Task<ActionResult<List<HttpBook>>> GetBooksByYearAsync(int startYear, int endYear)
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<HttpBook>>> BooksByUserAsync(int userId)
        {
            var checkedOutBooks = new List<HttpBook>();
            var books = await _repository.ListAsync(new UserSpecification(userId));

            foreach (var book in books)
            {
                checkedOutBooks.Add(new HttpBook
                {
                    Title = book.Title,
                    AuthorId = book.AuthorId,
                    BookId = book.BookId,
                    DewyClass = book.DewyClass,
                    Isbn = book.Isbn,
                    Year = book.Year,
                });
            }

            return Ok(checkedOutBooks);
        }
    }
}