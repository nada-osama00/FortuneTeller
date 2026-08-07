using System.Linq.Expressions;
using FortuneDomain.Common;
using FortuneDomain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FortuneInfrastructure.Repositories
{
    public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public IQueryable<T> Query() => DbSet.AsQueryable();

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await DbSet.FindAsync([id], cancellationToken);

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await DbSet.Where(predicate).ToListAsync(cancellationToken);

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await DbSet.SingleOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await DbSet.AnyAsync(predicate, cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
            await DbSet.AddAsync(entity, cancellationToken);

        public void Update(T entity) => DbSet.Update(entity);

        public void Remove(T entity) => DbSet.Remove(entity);
    }
}
