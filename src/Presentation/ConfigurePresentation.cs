using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Domain;
using Domain.Common.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Presentation;
using Presentation.Filters;
using ZymLabs.NSwag.FluentValidation;

[assembly: HostingStartup(typeof(ConfigurePresentation))]

namespace Presentation;

/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigurePresentation : IHostingStartup
{
    private static readonly string[] CompressionTypes = ["application/octet-stream"];

    /// <inheritdoc />
    public void Configure(IWebHostBuilder builder)
    {
        // general api services
        builder.ConfigureServices(services =>
        {
            services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("db");

            services.AddHttpContextAccessor();

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

            // services.AddSignalR(o => { o.EnableDetailedErrors = env.IsDevelopment(); });

            services.AddResponseCaching();
            services.AddResponseCompression(o =>
            {
                o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(CompressionTypes);
                o.Providers.Add<GzipCompressionProvider>();
                o.Providers.Add<BrotliCompressionProvider>();
            });
        });

        // front-end services
        builder.ConfigureServices(services =>
        {
            services.AddAntiforgery();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("WEB_APP__DOMAIN".FromEnv() ?? throw new Exception("WEB_APP__DOMAIN is not set in the environment"));
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
            });
        });

        // validation
        builder.ConfigureServices(services =>
        {
            services.AddValidatorsFromAssembly(DomainAssembly.Assembly, includeInternalTypes: true);
            services.AddValidatorsFromAssembly(ApplicationAssembly.Assembly, includeInternalTypes: true);
            services.AddValidatorsFromAssembly(InfrastructureAssembly.Assembly, includeInternalTypes: true);
            services.AddValidatorsFromAssembly(PresentationAssembly.Assembly, includeInternalTypes: true);

            services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            services.AddScoped<FluentValidationSchemaProcessor>(provider =>
            {
                var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
                var loggerFactory = provider.GetService<ILoggerFactory>();
                return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
            });
        });

        // swagger
        builder.ConfigureServices((context, services) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
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
                            Email = "g@ddjerqq.xyz",
                            Url = new Uri("https://github.com/ddjerqq/klean"),
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
        });
    }
}