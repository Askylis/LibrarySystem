using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.DataAccess
{
    public class LibraryContext : DbContext
    {
        private readonly string _connectionString;
        public LibraryContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public LibraryContext(DbContextOptions options) 
            : base(options)
        { 
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
