using SMS.Domain.Entities.Core;

namespace SMS.Domain.Interfaces.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        /// <summary>
        /// To get the student using emailid.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Student?> GetByEmailAsync(string email);

        /// <summary>
        /// To check if the same student code exists already.
        /// </summary>
        /// <param name="studentCode"></param>
        /// <returns></returns>
        Task<bool> ExistsByStudentCodeAsync(string studentCode);
    }
}
