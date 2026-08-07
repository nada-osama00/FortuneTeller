using System.Linq.Expressions;
using FortuneDomain.Common;

namespace FortuneDomain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>Escape hatch for callers that need Include/projection/paging beyond the helpers below.</summary>
        IQueryable<T> Query();

        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Remove(T entity);
    }
}
