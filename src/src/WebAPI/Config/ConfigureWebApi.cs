using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Domain.Abstractions;
using Domain.Common;
using FluentValidation.AspNetCore;
using Generated;
using Infrastructure.JsonConverters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;
using WebAPI.Filters;
using WebAPI.HealthChecks;
using ZymLabs.NSwag.FluentValidation;

namespace WebAPI.Config;

/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
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
                if (IsDevelopment)
                {
                    o.Filters.Add<ResponseTimeFilter>();
                }

                o.Filters.Add<SetClientIpAddressFilter>();
                o.Filters.Add<FluentValidationFilter>();
                o.RespectBrowserAcceptHeader = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new UlidToStringJsonConverter());
                options.JsonSerializerOptions.Converters.ConfigureGeneratedConverters();
            });

        services.AddCors(options =>
        {
            var webAppDomain = "WEB_APP__DOMAIN".FromEnvRequired();

            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader();
                policy.WithOrigins(webAppDomain);
                policy.WithMethods("GET", "POST", "HEAD");
                policy.AllowCredentials();
            });
        });

        services.AddSignalR(o => { o.EnableDetailedErrors = IsDevelopment; });

        services.AddResponseCaching();
        services.AddResponseCompression(o =>
        {
            o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(CompressionTypes);
            o.Providers.Add<GzipCompressionProvider>();
            o.Providers.Add<BrotliCompressionProvider>();
        });
    }
}