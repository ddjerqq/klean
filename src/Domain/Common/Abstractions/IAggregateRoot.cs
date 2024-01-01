using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Common.Abstractions;

/// <summary>
/// Represents the base AggregateRoot.
///
/// An aggregate is a cluster of entities and value objects
/// that belong together to form a consistency boundary.
///
/// For example, in a social media app, with posts, comments
/// and likes, the post would be the aggregate, because it
/// contains the comments and likes, and is a consistency
/// boundary for them.
///
/// An aggregate has a root entity that is the entry point
/// for all interactions with the aggregate. For example,
/// an order is the root entity of an aggregate that
/// contains order items, payments, and shipments.
/// </summary>
public interface IAggregateRoot : IEntity
{
    /// <summary>
    /// Gets all the domain events for this aggregate root.
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    public IEnumerable<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds the domain event.
    /// </summary>
    /// <param name="domainEvent">The event to add to the aggregate root.</param>
    public void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Removes all the domain event from an aggregate root.
    /// </summary>
    public void ClearDomainEvents();
}

/// <inheritdoc cref="Domain.Common.Abstractions.IAggregateRoot" />
public abstract class AggregateRootBase : EntityBase, IAggregateRoot
{
    [NotMapped]
    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <inheritdoc />
    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc />
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}