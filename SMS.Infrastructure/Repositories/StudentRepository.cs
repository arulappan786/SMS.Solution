using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories;
using SMS.Infrastructure.Persistence.Context;

namespace SMS.Infrastructure.Repositories
{
    public class StudentRepository(AppDbContext context) : GenericRepository<Student>(context), IStudentRepository
    {
        public async Task<bool> ExistsByStudentCodeAsync(string studentCode)
        {
            return await _context.Students
                .AsNoTracking()
                .AnyAsync(s => s.StudentCode == studentCode);
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<int> GetTotalStudentCountAsync()
        {
            return await _context.Students.CountAsync();
        }
    }
}