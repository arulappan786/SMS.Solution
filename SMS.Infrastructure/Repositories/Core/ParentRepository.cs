using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories.Core;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Core
{
    public class ParentRepository(AppDbContext dbContext) 
        : GenericRepository<Parent>(dbContext), IParentRepository
    {
        public async Task<Parent?> GetByContactNumberAsync(string phoneNumber)
        {
            return await dbContext.Parents.AsNoTracking()
                               .FirstOrDefaultAsync(p => p.PrimaryPhone == phoneNumber);
        }

        public async Task<Parent?> GetByIdentityIdAsync(Guid userId)
        {
            return await dbContext.Parents.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Parent>> GetByStudentIdAsync(Guid studentId)
        {
            return await dbContext.Parents.AsNoTracking()
                               .Where(p => p.StudentParents.Any(s => s.StudentId == studentId))
                               .ToListAsync();
        }
    }
}