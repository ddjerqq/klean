#pragma warning disable CS1591
using System.Reflection;
using Application;
using Generated;
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
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.MapType<Ulid>(() => new OpenApiSchema { Type = "string" });

            // configure source generated type conversions
            var idTypes = Domain.Domain.Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i == typeof(IStrongId)));
            foreach (var type in idTypes)
                options.MapType(type, () => new OpenApiSchema { Type = "string" });

            options.SupportNonNullableReferenceTypes();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "klean",
                Version = "v1",
                Description = "klean architecture API",
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