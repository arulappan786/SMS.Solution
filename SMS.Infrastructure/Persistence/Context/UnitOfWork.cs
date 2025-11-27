using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Domain.Interfaces.Repositories;

// Assuming AppDbContext is defined in this namespace or accessible here.
// AppDbContext would need to be a generic DbContext (like IdentityDbContext) or a standard DbContext.
// For this file to compile, we will assume AppDbContext is available.
// NOTE: Since this class uses AppDbContext, it belongs in the Infrastructure layer,
// while IUnitOfWork remains in the Domain layer, adhering to DIP.

namespace SMS.Infrastructure.Persistence.Context
{
    // The AppDbContext must be injected into the concrete UnitOfWork to control the transaction lifecycle.
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        // This holds the current active transaction for the scope.
        private IDbContextTransaction? _currentTransaction;

        /// <summary>
        /// Initiates a new database transaction using the underlying DbContext.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The active transaction handle (IDbContextTransaction).</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already active.");
            }

            _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
            return _currentTransaction;
        }

        /// <summary>
        /// Saves all pending changes to the database and commits the active transaction.
        /// If no explicit transaction was started, it implicitly commits changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Save all pending changes tracked by the context (includes all repositories).
                var result = await context.SaveChangesAsync(cancellationToken);

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }

                return result;
            }
            catch
            {
                // If saving or committing fails, ensure rollback occurs.
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
                throw; // Re-throw the exception for the business service to handle.
            }
            finally
            {
                // Dispose of the transaction object regardless of success or failure.
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Aborts the active transaction and discards all pending changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            // Discard any tracked changes (necessary if SaveChanges was never called)
            context.ChangeTracker.Entries().ToList().ForEach(entry => entry.State = EntityState.Detached);

            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }

            // Dispose of the transaction object.
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}