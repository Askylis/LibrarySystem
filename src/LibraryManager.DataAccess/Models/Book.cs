using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.DataAccess.Models
{
    public class Book : IDeletable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        public string Title { get; set; }
        public string AuthorName { get; set; }
        public int AuthorId { get; set; }
        public int? UserId { get; set; } = null;
        public int DewyClass {  get; set; }
        public string Isbn { get; set; }
        public int Year { get; set; }
        public DateTime? DueDate { get; set; } = null;
        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public virtual Author Author { get; set; }
        public virtual User? User { get; set; }
    }
}
