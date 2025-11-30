using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Domain.Interfaces.Repositories.Academic
{
    public interface IAcademicYearRepository : IGenericRepository<AcademicYear>
    {
        /// <summary>
        /// Returns true if there exists any entry that conflicts with the input data.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <param name="startDate">The start date to check.</param>
        /// <param name="endDate">The end date to check.</param>
        /// <param name="excludedId">Optional: The ID of the entity being updated to exclude from the check (Edit Mode).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<bool> ExistsAsync(string name,
                               DateOnly? startDate,
                               DateOnly? endDate,
                               CancellationToken cancellationToken,
                               Guid? excludedId = null);
        
    }
}
