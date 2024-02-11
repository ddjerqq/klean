using Application.Abstractions;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(ConfigurePersistence))]

namespace Infrastructure;

public class ConfigurePersistence : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<EntitySaveChangesInterceptor>();
            services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

            services.AddDbContext<AppDbContext>(o =>
            {
                if (context.HostingEnvironment.IsDevelopment())
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
