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