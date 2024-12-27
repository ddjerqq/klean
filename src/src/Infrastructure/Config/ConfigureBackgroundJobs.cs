using Application;
using Domain.Common;
using Infrastructure.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Infrastructure.Config;

public sealed class ConfigureBackgroundJobs : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddQuartz(config =>
        {
            var processOutboxMessageIntervalSeconds = int.Parse("OUTBOX__INTERVAL".FromEnv("10"));

            config
                .AddJob<ProcessOutboxMessagesBackgroundJob>(ProcessOutboxMessagesBackgroundJob.Key, job => job.StoreDurably())
                .AddTrigger(trigger => trigger
                    .ForJob(ProcessOutboxMessagesBackgroundJob.Key)
                    .WithSimpleSchedule(schedule => schedule
                        .WithInterval(TimeSpan.FromSeconds(processOutboxMessageIntervalSeconds))
                        .RepeatForever()));

            config
                .AddJob<DeleteDeletedUsersBackgroundJob>(DeleteDeletedUsersBackgroundJob.Key, job => job.StoreDurably())
                .AddTrigger(trigger => trigger
                    .ForJob(DeleteDeletedUsersBackgroundJob.Key)
                    .WithSimpleSchedule(schedule => schedule
                        .WithInterval(TimeSpan.FromHours(1))
                        .RepeatForever()));

            config.UseInMemoryStore();
        });

        services.AddQuartzHostedService();
    }
}