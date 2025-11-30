using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Infrastructure.Persistance.Context;
using SMS.Infrastructure.Repositories.Common;

namespace SMS.Infrastructure.Repositories.Academic
{
    public class AcademicYearRepository(AppDbContext dbContext)
        : GenericRepository<AcademicYear>(dbContext), IAcademicYearRepository
    {
        public async Task<bool> ExistsAsync(string name,
                                            DateOnly? startDate,
                                            DateOnly? endDate,
                                            CancellationToken cancellationToken,
                                            Guid? excludedId = null)
        {
            string lowerName = name.ToLower();

            var isDuplicate = await dbContext.AcademicYears
                .AsNoTracking()
                .AnyAsync(a =>
                    // 1. Exclusion Clause: Ignore the entity being updated if excludedId is present
                    (excludedId == null || a.Id != excludedId.Value) &&

                    // 2. Conflict Check: Look for matching Name OR matching Date Range
                    (
                        a.Name.ToLower() == lowerName ||
                        (a.StartDate == startDate && a.EndDate == endDate)
                    ),

                    cancellationToken);

            return isDuplicate;
        }
    }
}
