namespace Domain.Abstractions;

public abstract class Entity<TId>(TId id) : IEntity<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; set; } = id;

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}