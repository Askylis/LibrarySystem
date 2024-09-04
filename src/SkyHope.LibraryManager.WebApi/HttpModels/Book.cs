namespace SkyHope.LibraryManager.WebApi.HttpModels
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int DewyClass { get; set; }
        public string Isbn { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
    }
}
