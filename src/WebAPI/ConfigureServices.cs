using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using WebAPI.Filters;
using ZymLabs.NSwag.FluentValidation;

namespace WebAPI;

/// <summary>
/// Configures the services in the API layer
/// </summary>
public static class ConfigureServices
{
    private static readonly string[] CompressionTypes = ["application/octet-stream"];

    /// <summary>
    /// Adds Web API services to the service collection
    /// </summary>
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddCheck("db", () => HealthCheckResult.Healthy("good"));

        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields =
                HttpLoggingFields.RequestPath
                | HttpLoggingFields.ResponseStatusCode
                | HttpLoggingFields.RequestMethod
                | HttpLoggingFields.RequestQuery
                | HttpLoggingFields.RequestHeaders
                | HttpLoggingFields.ResponseHeaders;

            logging.RequestHeaders.Add("X-Idempotency-Key");
            logging.ResponseHeaders.Add("X-Client-IP");
            logging.ResponseHeaders.Add("X-Response-Time");

            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });

        services.Configure<RouteOptions>(x =>
        {
            x.LowercaseUrls = true;
            x.LowercaseQueryStrings = true;
            x.AppendTrailingSlash = false;
        });

        services
            .AddControllers(o =>
            {
                o.Filters.Add<SetClientIpAddressFilter>();
                o.Filters.Add<FluentValidationFilter>();
                o.Filters.Add<ResponseTimeFilter>();
                o.RespectBrowserAcceptHeader = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; });

        services.AddSignalR(o => { o.EnableDetailedErrors = env.IsDevelopment(); });

        services.AddValidatorsFromAssembly(DomainAssembly.Assembly);
        services.AddValidatorsFromAssembly(ApplicationAssembly.Assembly);

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.AddScoped<FluentValidationSchemaProcessor>(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();
            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        if (env.IsDevelopment())
            services.AddSwagger();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5000", "https://localhost:5001");
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });
        });

        services.AddResponseCaching();
        services.AddResponseCompression(o =>
        {
            o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(CompressionTypes);
            o.Providers.Add<GzipCompressionProvider>();
            o.Providers.Add<BrotliCompressionProvider>();
        });

        return services;
    }

    /// <summary>
    /// Applies all pending migrations
    /// </summary>
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureCreated();

        if (!dbContext.Database.IsInMemory() && dbContext.Database.GetPendingMigrations().Any())
            dbContext.Database.Migrate();

        return app;
    }

    /// <summary>
    /// Configures the web API middleware
    /// </summary>
    public static WebApplication ConfigureWebApiMiddleware(this WebApplication app)
    {
        app.UseHttpLogging();
        app.MapHealthChecks("/health");
        app.UseExceptionHandler("/error");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseHttpsRedirection();
        }

        // for front-end
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.UseIdempotency();

        // for front-end
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        return app;
    }

    /// <summary>
    /// Adds Swagger services to the service collection
    /// </summary>
    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SupportNonNullableReferenceTypes();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Klean",
                Version = "v1",
                Description = "Klean architecture web api",
                Contact = new OpenApiContact
                {
                    Name = "Klean",
                    Email = "ddjerqq@gmail.com",
                    Url = new Uri("https://github.com/ddjerqq"),
                },
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.OperationFilter<IdempotencyKeyOperationFilter>();
        });
    }
}