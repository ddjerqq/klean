using Application.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class DeleteDeletedUsersBackgroundJob(IAppDbContext dbContext) : IJob
{
    public static readonly JobKey Key = new("delete_deleted_users_background_job");

    public async Task Execute(IJobExecutionContext context)
    {
        var threeYearsAgo = DateTime.UtcNow.AddDays(-90);

        var unprocessedUsersCount = await dbContext
            .Users
            .IgnoreQueryFilters()
            .Where(u => u.Deleted < threeYearsAgo)
            .CountAsync();

        if (unprocessedUsersCount == 0)
            return;

        var users = await dbContext
            .Users
            .IgnoreQueryFilters()
            .Where(u => u.Deleted < threeYearsAgo)
            .OrderBy(m => m.Id)
            .ToListAsync(context.CancellationToken);

        Log.Logger.Information("deleting {Count} old users", users.Count);
        dbContext.Users.RemoveRange(users);
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}