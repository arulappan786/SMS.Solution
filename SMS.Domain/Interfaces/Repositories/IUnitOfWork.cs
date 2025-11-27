using Microsoft.EntityFrameworkCore.Storage;

namespace SMS.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Initiates a new database transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The active transaction handle (IDbContextTransaction).</returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all pending changes to the database and commits the active transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Aborts the active transaction and discards all pending changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}