using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess.Repositories;
using LibraryManager.DataAccess;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : Controller
    {
        private readonly LibraryRepository _repository;
        private readonly LibraryOptions _libraryOptions;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(LibraryRepository repository,
            LibraryOptions options,
            ILogger<AuthorController> logger)
        {
            _repository = repository;
            _libraryOptions = options;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> AddAuthorAsync(HttpModels.Author author)
        {
            var authorToAdd = new Author
            {
                Name = author.Name,
                DateOfBirth = author.DateOfBirth,
                Biography = author.Biography
            };

            try
            {
                await _repository.AddAsync<Author>(authorToAdd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while adding new author to the database.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _repository.SaveAsync();
            return Created();
        }

        [HttpDelete("{authorId}")]
        public async Task<ActionResult> DeleteAuthorAsync(int authorId)
        {
            var author = await _repository.FindAsync<Author>(authorId);
            if (author is null)
            {
                return NotFound();
            }

            _repository.Delete(author);

            try
            {
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while deleting author to the database.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpPatch("{authorId}")]
        public async Task<ActionResult> EditAuthorAsync(int authorId, HttpModels.AuthorUpdateDto update)
        {
            var authorToEdit = await _repository.FindAsync<Author>(authorId);
            if (authorToEdit is null)
            {
                return NotFound();
            }

            authorToEdit.Name = update.Name;
            authorToEdit.DateOfBirth = update.DateOfBirth;
            authorToEdit.Biography = update.Biography;

            try
            {
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating author.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}
