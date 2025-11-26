using SMS.Domain.Interfaces.Repositories;

namespace SMS.Infrastructure.Persistence.Context
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
