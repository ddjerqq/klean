using Generated;

namespace Domain.Abstractions;

public interface IAggregateRoot<TId> : IEntity<TId>
    where TId : struct, IStrongId, IEquatable<TId>
{
    public IEnumerable<IDomainEvent> DomainEvents { get; }
    public void AddDomainEvent(IDomainEvent domainEvent);
    public void ClearDomainEvents();
}