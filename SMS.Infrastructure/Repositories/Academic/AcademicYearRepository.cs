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
        /// <summary>
        /// Returns true if there exists any entry in the AcademicYear store with the same name and or with the startdate and enddate.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string name, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
        {
            string lowerName = name.ToLower();

            var isDuplicate = await dbContext.AcademicYears
                .AsNoTracking()
                .AnyAsync(a =>
                    a.Name.ToLower() == lowerName ||

                    (a.StartDate == startDate && a.EndDate == endDate),

                    cancellationToken);

            return isDuplicate;
        }

        //public async Task<(IEnumerable<AcademicYear> Items, int TotalCount)> GetAllPaginatedAsync(
        //    int pageNumber, int pageSize, CancellationToken cancellationToken)
        //{
        //    // Calculate how many records to skip
        //    int skip = (pageNumber - 1) * pageSize;

        //    // 1. Get the total count (needed for pagination metadata)
        //    var totalCount = await dbContext.AcademicYears.CountAsync(cancellationToken);

        //    // 2. Retrieve the specific page of items
        //    var items = await dbContext.AcademicYears
        //        .AsNoTracking()
        //        .OrderBy(s => s.Id) // Always apply ordering for consistent pagination
        //        .Skip(skip)
        //        .Take(pageSize)
        //        .ToListAsync(cancellationToken);

        //    return (items, totalCount);
        //}
    }
}
