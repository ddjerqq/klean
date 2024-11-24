using Application;
using Application.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config;

public sealed class ConfigureInfrastructure : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

        services.AddTransient<IJwtGenerator, JwtGenerator>();
    }
}