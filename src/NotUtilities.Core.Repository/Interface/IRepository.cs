using System;

namespace NotUtilities.Core.Repository.Interface
{
    /// <summary>
    /// Defines an asynchronous repository for entity management, automatically persisting changes. Inherits from ITransactionalRepository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type, must implement IEntity&lt;TKey&gt;.</typeparam>
    /// <typeparam name="TKey">Entity primary key type, must implement IComparable&lt;TKey&gt; and IEquatable&lt;TKey&gt;.</typeparam>
    public interface IRepository<TEntity, TKey> : ITransactionalRepository<TEntity, TKey>, IAsyncDisposable
        where TEntity : class, IEntity<TKey>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        // Inherits all members from ITransactionalRepository<TEntity, TKey> and IAsyncDisposable.
        // This interface emphasizes automatic changes committing.
    }
}
