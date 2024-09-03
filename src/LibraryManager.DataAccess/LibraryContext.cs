using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.DataAccess
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions options) 
            : base(options)
        { 
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
