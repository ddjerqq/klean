using System.Text.Json;
using Application.Common;
using Application.JsonConverters;
using Application.Services;
using Domain.Abstractions;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Events;
using SerilogTracing;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesBackgroundJob(IPublisher publisher, IAppDbContext dbContext) : IJob
{
    public static readonly JobKey Key = new("process_outbox_messages");
    private static readonly int MessagesPerBatch = int.Parse("OUTBOX__MESSAGES_PER_BATCH".FromEnv("20"));

    public async Task Execute(IJobExecutionContext context)
    {
        var unprocessedMessageCount = await dbContext
            .Set<OutboxMessage>()
            .CountAsync(m => m.ProcessedOn == null);

        if (unprocessedMessageCount == 0)
            return;

        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.Id)
            .Take(MessagesPerBatch)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonSerializer.Deserialize<IDomainEvent>(message.Content, ApplicationJsonConstants.Options.Value);

            if (domainEvent is null)
            {
                // fatal
                Log.Logger.Fatal("failed to deserialize message {@MessageId}", message.Id);
                continue;
            }

            using var activity = Log.Logger.StartActivity("Publish {@DomainEvent}", domainEvent);

            try
            {
                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                message.Error = ex.ToString();
                Log.Logger.Fatal("failed to process message {@MessageId}", message.Id);
                activity.Complete(LogEventLevel.Fatal, ex);
            }
            finally
            {
                message.ProcessedOn = DateTimeOffset.UtcNow;
                await Log.CloseAndFlushAsync();
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}