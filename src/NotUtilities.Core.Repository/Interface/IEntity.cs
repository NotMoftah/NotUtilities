using System;

namespace NotUtilities.Core.Repository.Interface
{
    /// <summary>
    /// Defines a generic entity with a specified type of identifier.
    /// </summary>
    /// <typeparam name="T">Type of the entity's identifier, must implement IComparable&lt;T&gt; and IEquatable&lt;T&gt;.</typeparam>
    public interface IEntity<T> where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the entity's identifier.
        /// </summary>
        T Id { get; set; }
    }

    /// <summary>
    /// Defines an entity with a long type identifier, extending the generic IEntity interface.
    /// </summary>
    public interface IEntity : IEntity<long>
    {
        // Inherits Id property of type long from IEntity<T>.
    }
}
