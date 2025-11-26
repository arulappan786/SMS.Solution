using SMS.Domain.Entities.Core;

namespace SMS.Domain.Interfaces.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {

        /// <summary>
        /// To check if the same student code exists already.
        /// </summary>
        /// <param name="studentCode"></param>
        /// <returns></returns>
        Task<bool> ExistsByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// To get the student using emailid.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);        

        /// <summary>
        /// To get the total student count, so that we can generate new student code.
        /// </summary>
        /// <returns></returns>
        Task<int> GetTotalStudentCountAsync(CancellationToken cancellationToken = default);
    }
}
