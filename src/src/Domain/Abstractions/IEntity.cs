namespace Domain.Abstractions;

public interface IEntity<TId> where TId : IEquatable<TId>
{
    public TId Id { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public static IEqualityComparer<Entity<TId>> IdEqualityComparer =>
        EqualityComparer<Entity<TId>>.Create((x, y) =>
            x is null
                ? y is null
                : y is not null
                  && x.GetType() == y.GetType()
                  && x.Id.Equals(y.Id));
}