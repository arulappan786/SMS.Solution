using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Domain.Interfaces.Repositories.Academic
{
    public interface IClassesRepository : IGenericRepository<Entities.Academic.Classes>
    {
        Task<bool> ExistsAsync(string name, Guid academicYearId, CancellationToken cancellationToken, Guid? excludedId = null);
    }
}
