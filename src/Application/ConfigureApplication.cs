using System.ComponentModel;
using Application;
using Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureApplication))]

namespace Application;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureApplication : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAutoMapper(Application.Assembly);
            services.AddValidatorsFromAssembly(Application.Assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Application.Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
            });
        });
    }
}