using System.ComponentModel;
using Infrastructure;
using Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Hosting;
using Quartz;

[assembly: HostingStartup(typeof(ConfigureBackgroundJobs))]

namespace Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureBackgroundJobs : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddQuartz(config =>
            {
                var jobKey = new JobKey("ProcessOutboxMessagesJob");

                config
                    .AddJob<ProcessOutboxMessagesBackgroundJob>(jobKey)
                    .AddTrigger(trigger => trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithInterval(TimeSpan.FromSeconds(10))
                            .RepeatForever()));
            });

            services.AddQuartzHostedService();
        });
    }
}
