namespace NotUtilities.Core.Repository.Interface
{
    /// <summary>
    /// Defines the base entity interface.
    /// </summary>
    public interface IEntity
    {

    }

    /// <summary>
    /// Defines a generic entity with a specified type of identifier.
    /// </summary>
    /// <typeparam name="T">Type of the entity's identifier, must implement IComparable&lt;T&gt; and IEquatable&lt;T&gt;.</typeparam>
    public interface IEntity<T> : IEntity where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the entity's identifier.
        /// </summary>
        T Id { get; set; }
    }
}
