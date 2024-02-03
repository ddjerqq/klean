using System.ComponentModel;
using Application;
using Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureApplication))]

namespace Application;

/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureApplication : IHostingStartup
{
    /// <inheritdoc />
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddValidatorsFromAssembly(ApplicationAssembly.Assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(ApplicationAssembly.Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddAutoMapper(mapper =>
            {
                mapper.AddMaps(ApplicationAssembly.Assembly);
            });
        });
    }
}