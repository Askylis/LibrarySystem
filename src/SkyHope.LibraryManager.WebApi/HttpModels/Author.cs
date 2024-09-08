using LibraryManager.DataAccess;

namespace SkyHope.LibraryManager.WebApi.HttpModels
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}