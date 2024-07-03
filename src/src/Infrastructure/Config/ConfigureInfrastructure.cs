using System.ComponentModel;
using Application;
using Application.Services;
using Infrastructure.Idempotency;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConfigureInfrastructure : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddIdempotency();
        services.AddMemoryCache();

        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
    }
}