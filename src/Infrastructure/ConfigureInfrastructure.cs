using System.ComponentModel;
using Application.Abstractions;
using Infrastructure;
using Infrastructure.Idempotency;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureInfrastructure))]

namespace Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureInfrastructure : IHostingStartup
{
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
