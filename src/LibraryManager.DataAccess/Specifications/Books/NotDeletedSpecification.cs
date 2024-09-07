using Ardalis.Specification;
using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Specifications.Books
{
    public sealed class NotDeletedSpecification : Specification<Book>
    {
        public NotDeletedSpecification()
        {
            Query.Where(b => !b.IsDeleted);
        }
    }
}
