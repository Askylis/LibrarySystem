using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using LibraryManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManager.DataAccess.Repositories
{
    public class LibraryRepository
    {
        private readonly LibraryContext _context;
        private readonly SpecificationEvaluator _specificationEvaluator;
        public LibraryRepository(LibraryContext context)
        {
            _context = context;
            _specificationEvaluator = SpecificationEvaluator.Default;
        }

        public async Task<T?> FirstOrDefaultAsync<T>(ISpecification<T> specification, CancellationToken token = default)
            where T : class
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<List<T>> ListAsync<T>(ISpecification<T> specification, CancellationToken token = default)
            where T : class
        {
            return await ApplySpecification(specification).ToListAsync().ConfigureAwait(false);
        }

        public async Task<T?> FindAsync<T>(int id)
            where T : class
        {
            return await _context.Set<T>().FindAsync(id).ConfigureAwait(false);
        }

        public void Delete<T>(T entity)
            where T : class, IDeletable
        {
            entity.IsDeleted = true;
        }

        public async Task SaveAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token).ConfigureAwait(false);
        }

        private IQueryable<T> ApplySpecification<T>(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
            where T : class
        {
            return _specificationEvaluator.GetQuery(_context.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
        }
    }
}
