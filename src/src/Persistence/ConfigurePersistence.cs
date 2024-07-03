using System.ComponentModel;
using Application;
using Application.Services;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Interceptors;

namespace Persistence;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigurePersistence : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EntitySaveChangesInterceptor>();
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<AppDbContext>(builder =>
        {
            if (IsDevelopment)
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
            }

            var dbPath = "DB__PATH".FromEnvRequired();
            builder.UseSqlite($"Data Source={dbPath}");
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        // delegate the IDbContext to the AppDbContext;
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    }
}
