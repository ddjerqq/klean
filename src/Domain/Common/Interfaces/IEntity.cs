namespace Domain.Common.Interfaces;

/// <summary>
/// Represents the base Entity.
///
/// An entity is an object that has a unique identity
/// and a lifecycle. It represents something that is
/// important for your business domain and that can
/// change over time. For example, a customer, an
/// order, or a product are entities.
/// </summary>
public interface IEntity : IEquatable<IEntity>
{
    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets the user id (if any) who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the user id (if any) who last modified the entity.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}

/// <inheritdoc />
public abstract class EntityBase : IEntity
{
    /// <inheritdoc />
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime? Created { get; set; }

    /// <inheritdoc />
    public string? CreatedBy { get; set; }

    /// <inheritdoc />
    public DateTime? LastModified { get; set; }

    /// <inheritdoc />
    public string? LastModifiedBy { get; set; }

    /// <inheritdoc />
    public bool Equals(IEntity? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }
}