using Application;
using Application.Services;
using Infrastructure.Idempotency;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

internal sealed class ConfigureInfrastructure : ConfigurationBase
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddIdempotency();
        services.AddMemoryCache();

        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
    }
}