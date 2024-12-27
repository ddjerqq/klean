using Application.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public sealed class ConfigureApplication : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(Application.Assembly);
        services.AddValidatorsFromAssembly(Application.Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Application.Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
        });
    }
}