using System.Linq.Expressions;

namespace SMS.Domain.Interfaces.Repositories.Common
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPaginatedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, TKey>> orderByExpression, bool ascending = true, CancellationToken cancellationToken = default);
        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPaginatedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, TKey>> orderByExpression, Expression<Func<TEntity, object>>[]? includeProperties = null, bool ascending = true, CancellationToken cancellationToken = default);
        Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetAsync(Guid id, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}