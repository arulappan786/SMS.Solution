using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Domain.Interfaces.Repositories.Academic
{
    public interface IAcademicYearRepository : IGenericRepository<AcademicYear>
    {
        Task<(IEnumerable<AcademicYear> Items, int TotalCount)> GetAllPaginatedAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(string name, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    }
}
