using Microsoft.EntityFrameworkCore;
using SMS.Domain.Interfaces.Repositories;
using SMS.Infrastructure.Persistence.Context;

namespace SMS.Infrastructure.Repositories
{
    public class GenericRepository<TEntity>(AppDbContext dbContext) : IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Generic repository method to add an entity to the underlying database entity/table.
        /// </summary>
        /// <param name="entity">This is the generic type that has to be passed by the caller</param>
        /// <returns>Returns true if added, false if not added.</returns>
        public async Task<bool> AddAsync(TEntity entity)
        {
            await dbContext.AddAsync(entity);
            var result = await dbContext.SaveChangesAsync();
            return (result > 0);
        }

        /// <summary>
        /// Generic repository method to delete an entity from the underlying database entity/table.
        /// </summary>
        /// <param name="id">The primary/unique key based on which the entity record has to be found and deleted.</param>
        /// <returns>Returns true if deleted, false if not deleted.</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            // Step1: Find the entity first prior to deleting it. if not found return false to the caller.
            var delEntity = await dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));

            if (delEntity == null) return false;

            // Step2: Delete the found entity. and save changes.
            var remResult = dbContext.Set<TEntity>().Remove(delEntity);
            var result = await dbContext.SaveChangesAsync();

            return (result > 0);
        }

        /// <summary>
        /// Generic repository method to retrieve all records from the underlying database entity/table.
        /// </summary>
        /// <returns>Returns collection of entity records.</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }


        /// <summary>
        /// Generic repository method to get an entity record from the underlying database entity/table.
        /// </summary>
        /// <param name="id">The primary/unique key based on which the entity record has to be found.</param>
        /// <returns>Returns a single record if found, otherwise empty type.</returns>
        public async Task<TEntity> GetAsync(Guid id)
        {
            var result = await dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
            return result!;
        }

        /// <summary>
        /// Generic repository method to update an entity record on the underlying database entity/table.
        /// </summary>
        /// <param name="id">The primary/unique key based on which the entity record has to be found.</param>
        /// <param name="entity">The entity itself whose values have to be updated.</param>
        /// <returns>Returns true if updated, false if not updated.</returns>
        public async Task<bool> UpdateAsync(Guid id, TEntity entity)
        {
            // Step1: Find the entity first prior to updating it. if not found return false to the caller.
            var upEntity = await dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
            if (upEntity == null) return false;

            // Step2: Update the entity with the suplied entity values.
            dbContext.Set<TEntity>().Update(entity);
            var result = await dbContext.SaveChangesAsync();

            return (result > 0);
        }

    }
}