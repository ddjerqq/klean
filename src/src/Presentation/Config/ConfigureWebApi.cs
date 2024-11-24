using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Domain.Common;
using FluentValidation.AspNetCore;
using Generated;
using Infrastructure.JsonConverters;
using Microsoft.AspNetCore.ResponseCompression;
using Persistence;
using Presentation.Filters;
using Presentation.HealthChecks;
using ZymLabs.NSwag.FluentValidation;

namespace Presentation.Config;

/// <inheritdoc />
public sealed class ConfigureWebApi : ConfigurationBase
{
    private static readonly string[] CompressionTypes = ["application/octet-stream"];

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddAntiforgery();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("db")
            .AddCheck<TestHealthCheck>("Test");

        services.AddScoped<GlobalExceptionHandlerMiddleware>();
        services.AddHttpContextAccessor();

        services.Configure<RouteOptions>(x =>
        {
            x.LowercaseUrls = true;
            x.LowercaseQueryStrings = true;
            x.AppendTrailingSlash = false;
        });

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.AddScoped<FluentValidationSchemaProcessor>(sp =>
        {
            var validationRules = sp.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = sp.GetService<ILoggerFactory>();
            return new FluentValidationSchemaProcessor(sp, validationRules, loggerFactory);
        });

        services
            .AddControllers(o =>
            {
                o.Filters.Add<ResponseTimeFilter>();
                o.Filters.Add<SetClientIpAddressFilter>();
                o.Filters.Add<FluentValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new UlidToStringJsonConverter());
                options.JsonSerializerOptions.Converters.ConfigureGeneratedConverters();
            });

        services.AddCors(options =>
        {
            var webAppDomain = "WEB_APP__API_DOMAIN".FromEnvRequired();

            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:1080", "https://localhost:1443", webAppDomain)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddResponseCaching();
        services.AddResponseCompression(o =>
        {
            o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(CompressionTypes);
            o.Providers.Add<GzipCompressionProvider>();
            o.Providers.Add<BrotliCompressionProvider>();
        });
    }
}