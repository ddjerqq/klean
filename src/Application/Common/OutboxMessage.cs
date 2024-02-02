using Application.Abstractions;
using Domain.Common.Interfaces;
using Newtonsoft.Json;

namespace Application.Common;

/// <summary>
/// The outbox message.
/// </summary>
public sealed class OutboxMessage
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy(),
        },
    };

    /// <summary>
    /// Gets the id for this message.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    public string Type { get; init; } = default!;

    /// <summary>
    /// Gets the content of the message.
    /// this is the event object serialized to json.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the date and time the message occured on.
    /// </summary>
    public DateTime OccuredOnUtc { get; init; }

    /// <summary>
    /// Gets or sets the date and time the message was processed on.
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the error message if the message failed to be processed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Creates a new <see cref="OutboxMessage"/> from a <see cref="IDomainEvent"/>.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <param name="dateTimeProvider">The date time.</param>
    /// <returns>A new <see cref="OutboxMessage"/>.</returns>
    public static OutboxMessage FromDomainEvent(IDomainEvent domainEvent, IDateTimeProvider dateTimeProvider)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = domainEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings),
            OccuredOnUtc = dateTimeProvider.UtcNow,
            ProcessedOnUtc = null,
            Error = null,
        };
    }
}