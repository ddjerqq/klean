using Application;
using Application.Services;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Infrastructure;

public class ConfigurePersistence : ConfigurationBase
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<EntitySaveChangesInterceptor>();
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<AppDbContext>(builder =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
            }

            builder.UseInMemoryDatabase("app");
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        // delegate the IDbContext to the AppDbContext;
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    }
}
