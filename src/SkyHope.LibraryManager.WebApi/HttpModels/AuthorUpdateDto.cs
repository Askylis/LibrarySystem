namespace SkyHope.LibraryManager.WebApi.HttpModels
{
    public class AuthorUpdateDto
    {
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
