using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.DataAccess.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }

        public string Name { get; set; }
        public string Biography { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
