using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Domain.Common;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.ResponseCompression;
using Presentation.Filters;

namespace Presentation;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class ConfigurePresentation : ConfigurationBase
{
    private static readonly string[] CompressionTypes = ["application/octet-stream"];

    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.AddAntiforgery();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("db");

        services.AddHttpContextAccessor();

        services.Configure<RouteOptions>(x =>
        {
            x.LowercaseUrls = true;
            x.LowercaseQueryStrings = true;
            x.AppendTrailingSlash = false;
        });

        services
            .AddControllers(o =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    o.Filters.Add<ResponseTimeFilter>();
                }

                o.Filters.Add<SetClientIpAddressFilter>();
                o.Filters.Add<FluentValidationFilter>();
                o.RespectBrowserAcceptHeader = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddCors(options =>
        {
            var webAppDomain = "WEB_APP__DOMAIN".FromEnv()
                               ?? throw new Exception("WEB_APP__DOMAIN is not set in the environment");

            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader();
                policy.WithOrigins(webAppDomain);
                policy.WithMethods("GET", "POST", "HEAD");
                policy.AllowCredentials();
            });
        });

        services.AddSignalR(o => { o.EnableDetailedErrors = context.HostingEnvironment.IsDevelopment(); });

        services.AddResponseCaching();
        services.AddResponseCompression(o =>
        {
            o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(CompressionTypes);
            o.Providers.Add<GzipCompressionProvider>();
            o.Providers.Add<BrotliCompressionProvider>();
        });
    }
}