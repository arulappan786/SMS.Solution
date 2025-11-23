using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Core;

namespace SMS.Application.Services.Interfaces.Context
{
    public interface IAppDbContext
    {
        // 1. Expose DbSets for the Domain Entities that need to be queried.
        // These are required by the Application and Repository layers.
        DbSet<Student> Students { get; }
        DbSet<Teacher> Teachers { get; }
        DbSet<Parent> Parents { get; }

        // 2. Define the core Unit of Work method: Save Changes.
        // This is the method the Application layer will call to commit the transaction.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}