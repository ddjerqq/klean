using System.ComponentModel;
using Application.Common.Interfaces;
using Infrastructure;
using Infrastructure.Idempotency;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureInfrastructure))]

namespace Infrastructure;

/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureInfrastructure : IHostingStartup
{
    /// <inheritdoc />
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddIdempotency();
            services.AddMemoryCache();
        });
    }
}
