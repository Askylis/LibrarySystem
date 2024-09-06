using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.DataAccess.Models
{
    public class User : IDeletable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public decimal LateFeeDue { get; set; } = 0m;
        public bool IsDeleted { get; set; }
        public virtual ICollection<Book> CheckedOutBooks { get; set; }
    }
}
