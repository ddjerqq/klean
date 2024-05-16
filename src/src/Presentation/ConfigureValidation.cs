using System.ComponentModel;
using Application;
using FluentValidation;
using FluentValidation.AspNetCore;
using ZymLabs.NSwag.FluentValidation;

namespace Presentation;

/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConfigureValidation : ConfigurationBase
{
    /// <inheritdoc />
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Domain.Domain.Assembly, includeInternalTypes: true);
        services.AddValidatorsFromAssembly(Application.Application.Assembly, includeInternalTypes: true);

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.AddScoped<FluentValidationSchemaProcessor>(sp =>
        {
            var validationRules = sp.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = sp.GetService<ILoggerFactory>();
            return new FluentValidationSchemaProcessor(sp, validationRules, loggerFactory);
        });
    }
}