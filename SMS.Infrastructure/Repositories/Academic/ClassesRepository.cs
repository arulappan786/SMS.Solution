using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Academic
{
    public class ClassesRepository(AppDbContext context) 
        : GenericRepository<Classes>(context), IClassesRepository
    {
        public async Task<bool> ExistsAsync(string name, Guid academicYearId, CancellationToken cancellationToken, Guid? excludedId = null)
        {
            string lowerName = name.ToLower();

            var isDuplicate = await context.Classes
                .AsNoTracking()
                .AnyAsync(c =>
                    // 1. Exclusion Clause: Ignore the entity being updated if excludedId is present
                    (excludedId == null || c.Id != excludedId.Value) &&
                    // 2. Conflict Check: Look for matching Name within the same Academic Year
                    (c.Name.ToLower() == lowerName && c.AcademicYearId == academicYearId),                     
                    cancellationToken);

            return isDuplicate;
        }
    }
}
