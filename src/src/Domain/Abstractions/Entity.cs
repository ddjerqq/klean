using Generated;

namespace Domain.Abstractions;

public abstract class Entity<TId>(TId id) : IEntity<TId>
    where TId : struct, IStrongId, IEquatable<TId>
{
    public TId Id { get; set; } = id;

    public DateTimeOffset? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}