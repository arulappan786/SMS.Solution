using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Infrastructure.Persistance.Context;

namespace SMS.Infrastructure.Repositories
{
    public class ParentRepository(AppDbContext context) : GenericRepository<Parent>(context), IParentRepository
    {
        public async Task<Parent?> GetByContactNumberAsync(string phoneNumber)
        {
            return await context.Parents.AsNoTracking()
                               .FirstOrDefaultAsync(p => p.PrimaryPhone == phoneNumber);
        }

        public async Task<Parent?> GetByIdentityIdAsync(Guid userId)
        {
            return await context.Parents.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Parent>> GetByStudentIdAsync(Guid studentId)
        {
            return await context.Parents.AsNoTracking()
                               .Where(p => p.StudentParents.Any(s => s.StudentId == studentId))
                               .ToListAsync();
        }
    }
}