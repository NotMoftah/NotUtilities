using System;
using System.Linq.Expressions;

namespace NotUtilities.Core.Repository.Interface
{
    /// <summary>
    /// Defines an asynchronous repository for managing entities of type TEntity. 
    /// Requires transactions for changes persistence.
    /// </summary>
    /// <typeparam name="TEntity">Entity type, must implement IEntity&lt;TKey&gt;.</typeparam>
    /// <typeparam name="TKey">Entity primary key type, must implement IComparable&lt;TKey&gt; and IEquatable&lt;TKey&gt;.</typeparam>
    public interface IRepository<TEntity, TKey> : IAsyncDisposable
        where TEntity : class, IEntity<TKey>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        /// <summary>
        /// Exposes a queryable collection of entities.
        /// </summary>
        IQueryable<TEntity> Entities { get; }

        /// <summary>
        /// Fetches an entity by primary key asynchronously.
        /// </summary>
        /// <param name="id">Entity primary key.</param>
        /// <returns>The entity or null if not found.</returns>
        Task<TEntity?> FindByIdAsync(TKey id);

        /// <summary>
        /// Fetches an entity that matches the given predicate asynchronously.
        /// </summary>
        /// <param name="predicate">Criteria to filter entities.</param>
        /// <returns>The entity or null if not found.</returns>
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Fetches entities by primary keys asynchronously.
        /// </summary>
        /// <param name="ids">Entity primary keys.</param>
        /// <returns>A collection of entities.</returns>
        Task<IEnumerable<TEntity>> FindByIdsAsync(IEnumerable<TKey> ids);

        /// <summary>
        /// Fetches entities that match the given predicate asynchronously.
        /// </summary>
        /// <param name="predicate">Criteria to filter entities.</param>
        /// <returns>A collection of entities.</returns>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Projects entities to a new form based on a given query asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="selector">A transform function to apply.</param>
        /// <returns>A collection of projected elements.</returns>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> selector);

        /// <summary>
        /// Inserts an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>Success flag.</returns>
        ValueTask<bool> InsertAsync(TEntity entity);

        /// <summary>
        /// Inserts multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        /// <returns>Success flag.</returns>
        ValueTask<bool> InsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>Success flag.</returns>
        ValueTask<bool> UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities to update.</param>
        /// <returns>Success flag.</returns>
        ValueTask<bool> UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity by primary key asynchronously.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>Success flag.</returns>
        ValueTask<bool> DeleteAsync(TEntity entity);

        /// <summary>
        /// Deletes multiple entities by primary keys asynchronously.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <returns>Success flag.</returns>
        ValueTask<bool> DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Persists pending changes to the database asynchronously.
        /// </summary>
        /// <returns>Task for operation completion.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Discard pending changes to the database asynchronously.
        /// </summary>
        /// <returns>Task for operation completion.</returns>
        ValueTask DiscardChangesAsync();

    }
}
