using SMS.Domain.Entities.Core;

namespace SMS.Domain.Interfaces.Repositories
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
