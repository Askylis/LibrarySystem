using Ardalis.Specification;
using LibraryManager.DataAccess.Models;

namespace LibraryManager.DataAccess.Specifications.Books
{
    public sealed class YearSpecification : Specification<Book>
    {
        public YearSpecification(int startYear, int endYear)
        {
            Query.Where(b => b.Year <= endYear && b.Year >= startYear);
        }
    }
}
