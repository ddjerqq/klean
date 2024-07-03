using System.ComponentModel;
using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WebAPI.Swagger;

namespace WebAPI.Config;

/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConfigureSwagger : ConfigurationBase
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        if (!IsDevelopment)
            return;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            const string xmlFile = $"{nameof(WebAPI)}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.OperationFilter<IdempotencyKeyOperationFilter>();
            options.OperationFilter<RequestedWithXmlHttpRequest>();
            options.SchemaFilter<StrongIdSchemaFilter>();

            options.SupportNonNullableReferenceTypes();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ruby",
                Version = "v1",
                Description = "Digital casino with cases, items and blockchain!",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    []
                },
            });
        });
    }
}