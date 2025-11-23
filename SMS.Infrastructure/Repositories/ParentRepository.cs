using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;
using SMS.Infrastructure.Persistence.Context;

namespace SMS.Infrastructure.Repositories
{
    public class ParentRepository(AppDbContext context) : GenericRepository<Parent>(context), IParentRepository
    {
        public async Task<Parent?> GetByContactNumberAsync(string phoneNumber)
        {
            return await _dbSet.AsNoTracking()
                               .FirstOrDefaultAsync(p => p.PrimaryPhone == phoneNumber);
        }

        public async Task<Parent?> GetByIdentityIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Parent>> GetByStudentIdAsync(int studentId)
        {
            return await _dbSet.AsNoTracking()
                               .Where(p => p.StudentParents.Any(s => s.StudentId == studentId))
                               .ToListAsync();
        }
    }
}