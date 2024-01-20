using Application.Common.Interfaces;
using Domain.Common.Extensions;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigurePersistence))]

namespace Infrastructure;

/// <inheritdoc />
public class ConfigurePersistence : IHostingStartup
{
    /// <inheritdoc />
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<EntitySaveChangesInterceptor>();
            services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

            services.AddDbContext<AppDbContext>(o =>
            {
                if ("ASPNETCORE_ENVIRONMENT".FromEnv() is "Development")
                {
                    o.EnableDetailedErrors();
                    o.EnableSensitiveDataLogging();
                }

                o.UseInMemoryDatabase("app");
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            // delegate the IDbContext to the AppDbContext;
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        });
    }
}
