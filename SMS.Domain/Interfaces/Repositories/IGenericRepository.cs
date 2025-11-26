namespace SMS.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(Guid id);

        Task<bool> AddAsync(TEntity entity);

        Task<bool> UpdateAsync(Guid id, TEntity entity);

        Task<bool> DeleteAsync(Guid id);
    }
}
