using Application.Abstractions;
using Application.Common;
using Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Infrastructure.BackgroundJobs;

/// <summary>
/// The <see cref="ProcessOutboxMessagesBackgroundJob" /> class.
/// </summary>
[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesBackgroundJob(
    IPublisher publisher,
    IAppDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProcessOutboxMessagesBackgroundJob> logger)
    : IJob
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
    /// Executes the job.
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccuredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(message.Content, JsonSerializerSettings);

            if (domainEvent is null)
            {
                logger.LogWarning("Failed to deserialize message {MessageId}", message.Id);
                continue;
            }

            try
            {
                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                message.Error = ex.ToString();
                logger.LogError(ex, "Failed to publish message {MessageId}", message.Id);
            }

            message.ProcessedOnUtc = dateTimeProvider.UtcNow;
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}