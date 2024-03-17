using System;
using System.Transactions;

namespace NotUtilities.Core.Repository.Interface
{
    /// <summary>
    /// Defines the unit of work for managing transactions and repositories.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Provides access to a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        IRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class, IEntity<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>;

        /// <summary>
        /// Persists all pending changes asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Discard all pending changes asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        ValueTask DiscardChangesAsync();

        /// <summary>
        /// Starts a new transaction asynchronously with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">The isolation level of the transaction. Default is ReadCommitted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Commits the current transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RollbackTransactionAsync();
    }
}
