using System.ComponentModel;
using Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class ConfigureApplication : ConfigurationBase
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.AddAutoMapper(Application.Assembly);
        services.AddValidatorsFromAssembly(Application.Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Application.Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
        });
    }
}