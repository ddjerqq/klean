#pragma warning disable CS1591
using System.Reflection;
using Application;
using Microsoft.OpenApi.Models;

namespace Presentation.Config;

public sealed class ConfigureSwagger : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        if (!IsDevelopment)
            return;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
            options.SupportNonNullableReferenceTypes();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "klean",
                Version = "v1",
                Description = "klean API",
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