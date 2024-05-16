using Application;
using Application.Services.Interfaces;
using Infrastructure.Idempotency;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class ConfigureInfrastructure : ServiceConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddIdempotency();
        services.AddMemoryCache();

        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
    }
}