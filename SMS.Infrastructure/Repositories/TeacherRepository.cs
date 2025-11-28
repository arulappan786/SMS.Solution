using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories;
using SMS.Infrastructure.Persistance.Context;

namespace SMS.Infrastructure.Repositories
{
    public class TeacherRepository(AppDbContext context) : GenericRepository<Teacher>(context), ITeacherRepository
    {
        public async Task<Teacher?> GetByEmailAsync(string email)
        {
            return await context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Teacher?> GetByTeacherCodeAsync(string teacherCode)
        {
            return await context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.TeacherCode == teacherCode);
        }
    }
}
