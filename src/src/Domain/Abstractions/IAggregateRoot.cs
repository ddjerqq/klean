namespace Domain.Abstractions;

public interface IAggregateRoot<TId> : IEntity<TId>
    where TId : IEquatable<TId>
{
    public IEnumerable<IDomainEvent> DomainEvents { get; }

    public void AddDomainEvent(IDomainEvent domainEvent);

    public void ClearDomainEvents();
}