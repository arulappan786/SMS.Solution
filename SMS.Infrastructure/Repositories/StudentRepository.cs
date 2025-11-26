using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories;
using SMS.Infrastructure.Persistence.Context;

namespace SMS.Infrastructure.Repositories
{
    public class StudentRepository(AppDbContext context) : GenericRepository<Student>(context), IStudentRepository
    {
        public async Task<bool> ExistsByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default)
        {
            return await context.Students
                .AsNoTracking()
                .AnyAsync(s => s.StudentCode == studentCode, cancellationToken);
        }

        public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
        }

        public async Task<int> GetTotalStudentCountAsync(CancellationToken cancellationToken = default)
        {
            return await context.Students.CountAsync(cancellationToken);
        }
    }
}