using System.Reflection;
using Application;
using Microsoft.OpenApi.Models;
using Presentation.Swagger;

namespace Presentation;

public class ConfigureSwagger : ConfigurationBase
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        if (!context.HostingEnvironment.IsDevelopment())
            return;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.OperationFilter<IdempotencyKeyOperationFilter>();
            options.OperationFilter<RequestedWithXmlHttpRequest>();

            options.SupportNonNullableReferenceTypes();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Klean",
                Version = "v1",
                Description = "A ready-to-use template for ASP.NET Core with Domain Driven Design - clean architecture. " +
                              "Mediator, CQRS, StrongID patterns, Blazor WASM front-end, and much more!",
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