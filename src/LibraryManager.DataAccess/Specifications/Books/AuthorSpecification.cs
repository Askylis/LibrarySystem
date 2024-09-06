using Ardalis.Specification;
using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Specifications.Books
{
    public class AuthorSpecification : Specification<Book>
    {
        public AuthorSpecification(int authorId) 
        { 
            Query.Where(b => b.AuthorId == authorId);
        }
    }
}
