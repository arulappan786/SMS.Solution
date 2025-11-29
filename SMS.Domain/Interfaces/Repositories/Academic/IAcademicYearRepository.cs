using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Domain.Interfaces.Repositories.Academic
{
    public interface IAcademicYearRepository : IGenericRepository<AcademicYear>
    {
        Task<bool> ExistsAsync(string name, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    }
}
