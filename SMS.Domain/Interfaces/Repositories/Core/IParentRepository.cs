using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Domain.Interfaces.Repositories.Core
{
    public interface IParentRepository : IGenericRepository<Parent>
    {
        /// <summary>
        /// Get parent using the app userid.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Parent?> GetByIdentityIdAsync(Guid userId);

        /// <summary>
        /// Get all parents associated with a specific student.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        Task<IEnumerable<Parent>> GetByStudentIdAsync(Guid studentId);

        /// <summary>
        /// Get a parent using the primary phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<Parent?> GetByContactNumberAsync(string phoneNumber);
    }
}