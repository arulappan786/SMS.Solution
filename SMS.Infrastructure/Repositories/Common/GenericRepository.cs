using Microsoft.EntityFrameworkCore;

using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Infrastructure.Persistance.Context;
using System.Linq.Expressions;

namespace SMS.Infrastructure.Repositories.Common
{
    public class GenericRepository<TEntity>(AppDbContext dbContext) 
        : IGenericRepository<TEntity> where TEntity : class
    {
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
            var delEntity = await dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (delEntity == null) return false;
            dbContext.Set<TEntity>().Remove(delEntity);
            return true;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPaginatedAsync<TKey>(int pageNumber,
                                                                                                   int pageSize,
                                                                                                   Expression<Func<TEntity, TKey>> orderByExpression,
                                                                                                   bool ascending = true,
                                                                                                   CancellationToken cancellationToken = default)    
        {
            // Calculate how many records to skip
            int skip = (pageNumber - 1) * pageSize;

            // 1. Prepare the queryable
            IQueryable<TEntity> query = dbContext.Set<TEntity>().AsNoTracking();

            // 2. Get the total count
            var totalCount = await query.CountAsync(cancellationToken);

            // 3. Apply ordering dynamically
            if (ascending)
            {
                query = query.OrderBy(orderByExpression);
            }
            else
            {
                query = query.OrderByDescending(orderByExpression);
            }

            // 4. Apply pagination and retrieve items
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPaginatedAsync<TKey>(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, TKey>> orderByExpression,
            Expression<Func<TEntity, object>>[]? includeProperties = null, // New optional parameter for includes
            bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            // Calculate how many records to skip
            int skip = (pageNumber - 1) * pageSize;

            // 1. Prepare the queryable
            IQueryable<TEntity> query = dbContext.Set<TEntity>().AsNoTracking();

            // NEW: Apply includes if any are provided
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            // 2. Get the total count (must be counted before Skip/Take, but after Includes if using filtering)
            var totalCount = await query.CountAsync(cancellationToken);

            // 3. Apply ordering dynamically
            if (ascending)
            {
                query = query.OrderBy(orderByExpression);
            }
            else
            {
                query = query.OrderByDescending(orderByExpression);
            }

            // 4. Apply pagination and retrieve items
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
            return await dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id), cancellationToken);
        }

        public async Task<bool> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));           
            ArgumentNullException.ThrowIfNull(entity);
            var upEntity = await dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (upEntity == null) return false;
            dbContext.Entry(upEntity).CurrentValues.SetValues(entity);            
            return true;
        }
    }
}