using Ardalis.Specification;
using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Specifications.Users
{
    public class NotDeletedSpecification : Specification<User>
    {
        public NotDeletedSpecification() 
        {
            Query.Where(u => !u.IsDeleted);
        }
    }
}
