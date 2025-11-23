using Microsoft.EntityFrameworkCore;
using SMS.Domain.Interfaces.Repositories;
using SMS.Infrastructure.Persistence.Context;
using System.Linq.Expressions;

namespace SMS.Infrastructure.Repositories
{
    // The class is made abstract so that it cannot be instantiated directly, 
    // forcing specific repositories to inherit from it.
    // It is often made internal to prevent access outside the Infrastructure assembly.
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // 1. Dependency Injection: The DbContext is injected into the repository.
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        // Constructor to receive the DbContext instance
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Get the DbSet specific to the entity T
        }

        // --- C R E A T E ---
        public async Task AddAsync(T entity)
        {
            // Tracks the entity in the context with 'Added' state. 
            // Changes are saved when the Unit of Work (DbContext.SaveChanges) is called.
            await _dbSet.AddAsync(entity);
        }

        // --- R E A D : By ID ---
        public async Task<T?> GetByIdAsync(int id)
        {
            // Finds an entity with the given primary key value.
            // This is efficient and first checks the context cache.
            return await _dbSet.FindAsync(id);
        }

        // --- R E A D : All ---
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Retrieves all entities. AsNoTracking can improve performance for read-only scenarios.
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        // --- R E A D : Conditional ---
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            // Filters the DbSet using the provided LINQ expression (WHERE clause).
            return await _dbSet
                .AsNoTracking() // Recommended for read operations
                .Where(expression)
                .ToListAsync();
        }

        // --- U P D A T E ---
        public void Update(T entity)
        {
            // Attaches the entity and marks it as Modified.
            // If the entity is already tracked, it only marks its state as Modified.
            _dbSet.Update(entity);

        }

        // --- D E L E T E ---
        public void Remove(T entity)
        {
            // Marks the entity for deletion.
            _dbSet.Remove(entity);
        }
    }
}