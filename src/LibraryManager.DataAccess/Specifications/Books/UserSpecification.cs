using Ardalis.Specification;
using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Specifications.Books
{
    public class UserSpecification : Specification<Book>
    {
        public UserSpecification(int userId) 
        { 
            Query.Where(b => b.UserId == userId);
        }
    }
}
