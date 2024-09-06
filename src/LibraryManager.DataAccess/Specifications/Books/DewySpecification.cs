using Ardalis.Specification;
using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Specifications.Books
{
    public sealed class DewySpecification : Specification<Book>
    {
        public DewySpecification(int dewyClass)
        {
            Query.Where(b => b.DewyClass == dewyClass);
        }
    }
}
