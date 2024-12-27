using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.JsonConverters;
using Destructurama.Attributed;
using Domain.Abstractions;
using EntityFrameworkCore.DataProtection;

namespace Application.Common;

public sealed class OutboxMessage
{
    public Ulid Id { get; init; } = Ulid.NewUlid();

    [StringLength(128)]
    public string Type { get; init; } = null!;

    [LogMasked]
    [Encrypt(false, false)]
    [StringLength(2048)]
    public string Content { get; init; } = string.Empty;

    public DateTimeOffset OccuredOn { get; init; }

    public DateTimeOffset? ProcessedOn { get; set; }

    [StringLength(1024)]
    public string? Error { get; set; }

    public static OutboxMessage FromDomainEvent(IDomainEvent domainEvent)
    {
        return new OutboxMessage
        {
            Id = Ulid.NewUlid(),
            Type = domainEvent.GetType().AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(domainEvent, ApplicationJsonConstants.Options.Value),
            OccuredOn = DateTimeOffset.UtcNow,
            ProcessedOn = null,
            Error = null,
        };
    }
}