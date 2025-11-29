using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories.Core;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Core
{
    public class StudentRepository(AppDbContext dbContext) 
        : GenericRepository<Student>(dbContext), IStudentRepository
    {
        public async Task<bool> ExistsByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(studentCode, nameof(studentCode));
            return await dbContext.Students
                .AsNoTracking()
                .AnyAsync(s => s.StudentCode == studentCode, cancellationToken);
        }

        // SMS.Infrastructure.Repositories/StudentRepository.cs (EF Core Implementation)
        public async Task<(IEnumerable<Student> Items, int TotalCount)> GetAllPaginatedAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            // Calculate how many records to skip
            int skip = (pageNumber - 1) * pageSize;

            // 1. Get the total count (needed for pagination metadata)
            var totalCount = await dbContext.Students.CountAsync(cancellationToken);

            // 2. Retrieve the specific page of items
            var items = await dbContext.Students
                .AsNoTracking()
                .OrderBy(s => s.StudentCode) // Always apply ordering for consistent pagination
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(email, nameof(email));
            return await dbContext.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
        }

        public async Task<int> GetTotalStudentCountAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Students.CountAsync(cancellationToken);
        }
    }
}