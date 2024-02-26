namespace Domain.Common.Interfaces;

public abstract class EntityBase
{
    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}

public abstract class EntityBase<TId>(TId id) : EntityBase
    where TId : IEquatable<TId>
{
    public TId Id { get; init; } = id;

    public static IEqualityComparer<EntityBase<TId>> IdEqualityComparer =>
        EqualityComparer<EntityBase<TId>>.Create((x, y) =>
            x is null ? y is null
                : y is not null && x.GetType() == y.GetType() && x.Id.Equals(y.Id));
}