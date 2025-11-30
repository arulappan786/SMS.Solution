using Microsoft.AspNetCore.Identity;
using SMS.Application.CQRS.Core.Students.Commands.Create;
using SMS.Domain.Entities.Identity;
using System.Threading;

namespace SMS.Application.Services.Core.Students
{
    // This interface groups together the critical onboarding functions that span multiple repositories (User, Student).
    public interface IStudentOnboardingService
    {
        /// <summary>
        /// Checks if a student is already registered in EITHER the application user store OR the student entity store.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the student is unique, False otherwise.</returns>
        Task<bool> IsUniqueAsync(string email, CancellationToken cancellationToken);

        /// <summary>
        /// Atomically creates the AppUser and assigns the 'Student' role within a single transaction.
        /// </summary>
        /// <param name="studentCommand">The student data command.</param>
        /// <param name="password">The generated temporary password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newly created AppUser entity.</returns>
        /// <exception cref="InvalidOperationException">Throws if user creation or role assignment fails.</exception>
        Task<AppUser> CreateUserAndAssignRoleAsync(CreateStudentCommand studentCommand, string password, CancellationToken cancellationToken);

        Task<bool> LinkStudentProfileToUserAsync(AppUser user, Guid studentProfileId, CancellationToken cancellationToken);
    }
}