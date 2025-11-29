using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories.Core;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Core
{
    public class TeacherRepository(AppDbContext dbContext) 
        : GenericRepository<Teacher>(dbContext), ITeacherRepository
    {
        public async Task<Teacher?> GetByEmailAsync(string email)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(email, nameof(email));
            return await dbContext.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Teacher?> GetByTeacherCodeAsync(string teacherCode)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(teacherCode, nameof(teacherCode));
            return await dbContext.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.TeacherCode == teacherCode);
        }
    }
}
