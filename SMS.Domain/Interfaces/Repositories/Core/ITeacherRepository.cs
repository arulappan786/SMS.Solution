using SMS.Domain.Entities.Core;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Domain.Interfaces.Repositories.Core
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        /// <summary>
        /// Retrieve a teacher by their unique official Staff ID.
        /// </summary>
        /// <param name="teacherCode"></param>
        /// <returns></returns>
        Task<Teacher?> GetByTeacherCodeAsync(string teacherCode);

        /// <summary>
        /// Retrieve a teacher by their unique professional email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Teacher?> GetByEmailAsync(string email);
    }
}
