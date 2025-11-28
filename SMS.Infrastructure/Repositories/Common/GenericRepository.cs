using Microsoft.EntityFrameworkCore;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Infrastructure.Persistance.Context;

namespace SMS.Infrastructure.Repositories.Common
{
    public class GenericRepository<TEntity>(AppDbContext dbContext) 
        : IGenericRepository<TEntity> where TEntity : class
    {
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var delEntity = await dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (delEntity == null) return false;
            dbContext.Set<TEntity>().Remove(delEntity);
            return true;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id), cancellationToken);
        }

        public async Task<bool> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
        {
            var upEntity = await dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (upEntity == null) return false;
            dbContext.Entry(upEntity).CurrentValues.SetValues(entity);
            return true;
        }
    }
}