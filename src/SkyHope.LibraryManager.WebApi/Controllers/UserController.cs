using Microsoft.AspNetCore.Mvc;
using LibraryManager.DataAccess.Repositories;
using LibraryManager.DataAccess;
using Microsoft.Extensions.Options;
using LibraryManager.DataAccess.Models;
using LibraryManager.DataAccess.Specifications.Users;

namespace SkyHope.LibraryManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly LibraryRepository _repository;
        private readonly LibraryOptions _libraryOptions;
        private readonly ILogger<UserController> _logger;
        public UserController(LibraryRepository repository,
            ILogger<UserController> logger,
            IOptions<LibraryOptions> libraryOptions)
        {
            _repository = repository;
            _libraryOptions = libraryOptions.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HttpModels.User>>> GetAllAsync()
        {
            var results = new List<HttpModels.User>();
            var allUsers = await _repository.ListAsync(new NotDeletedSpecification());

            foreach (var user in allUsers)
            {
                results.Add(new HttpModels.User
                {
                    Address = user.Address,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    UserId = user.UserId
                });
            }

            _logger.LogInformation($"GetAllAsync in UserController returned {results.Count} users.");
            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult> AddUserAsync(HttpModels.User user)
        {
            var userToAdd = new HttpModels.User
            {
                Address = user.Address,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                UserId = user.UserId
            };

            try
            {
                await _repository.AddAsync(userToAdd);
            }
            catch
            {
                _logger.LogError("Something went wrong when adding a new user to the database.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            await _repository.SaveAsync();
            return Created();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUserAsync(int userId)
        {
            var user = await _repository.FindAsync<User>(userId);

            if (user is null)
            {
                return NotFound();
            }

            _repository.Delete(user);
            user.Name = _libraryOptions.AnonName;
            user.PhoneNumber = _libraryOptions.AnonPhoneNumber;
            user.Address = _libraryOptions.AnonAddress;
            await _repository.SaveAsync();

            return Ok();
        }

        [HttpPatch("{userId}")]
        public async Task<ActionResult> EditUserAsync(int userId, HttpModels.UserUpdateDto update)
        {
            var userToEdit = await _repository.FindAsync<User>(userId);
            if (userToEdit is null)
            {
                return NotFound();
            }

            userToEdit.PhoneNumber = update.PhoneNumber;
            userToEdit.Address = update.Address;
            userToEdit.LateFeeDue = update.LateFeeDue;
            try
            {
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Unexpected error when updating user.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}
