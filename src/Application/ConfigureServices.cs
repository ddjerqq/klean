using System.Reflection;
using Application.Common.Behaviours;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// The <see cref="ConfigureServices" /> class.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Adds the application services to the service collection.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(DomainAssembly.Assembly);
        services.AddValidatorsFromAssembly(ApplicationAssembly.Assembly);

        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(ApplicationAssembly.Assembly); });
        services.AddMediatR(
            cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
            });

        return services;
    }
}