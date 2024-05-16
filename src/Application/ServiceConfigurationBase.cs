using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public abstract class ServiceConfigurationBase : IHostingStartup
{
    protected bool Configured { get; set; }

    public abstract void ConfigureServices(IServiceCollection services);

    public virtual void Configure(IWebHostBuilder builder)
    {
        if (!Configured)
        {
            builder.ConfigureServices(ConfigureServices);
            Configured = true;
        }
    }
}