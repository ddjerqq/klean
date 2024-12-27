using System.Security.Claims;
using Application;
using Application.Common;
using Destructurama;
using Domain.Common;
using Generated;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Templates.Themes;
using SerilogTracing.Expressions;

namespace Infrastructure.Config;

public sealed class ConfigureLogging : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .Configure()
            .CreateLogger();

        services.AddSerilog();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, true));
    }
}

public static class LoggingExt
{
    public static LoggerConfiguration Configure(this LoggerConfiguration config)
    {
        SelfLog.Enable(Console.Error);

        var seqHost = "SEQ__HOST".FromEnvRequired();
        var seqApiKey = "SEQ__API_KEY".FromEnvRequired();

        return config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Destructure.UsingAttributes()
            .Destructure.ByTransforming<Ulid>(id => id.ToString())
            .Destructure.ByTransforming<IStrongId>(id => id.ToString()!)
            .Enrich.WithProperty("Application", "SEQ__APP_NAME".FromEnvRequired())
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithAssemblyName()
            .WriteTo.Debug()
            .WriteTo.Console(Formatters.CreateConsoleTextFormatter(TemplateTheme.Code))
            .WriteTo.Seq(seqHost, apiKey: seqApiKey);
    }

    public static void UseConfiguredSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.Logger = Log.Logger;
            options.IncludeQueryInRequestPath = true;
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("UserId", httpContext.User.FindFirstValue(ClaimsPrincipalExt.IdClaimType) ?? "unauthenticated");
                diagnosticContext.Set("ClientAddress", httpContext.Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
                diagnosticContext.Set("ClientUserAgent", (string?)httpContext.Request.Headers.UserAgent);
                diagnosticContext.Set("TraceIdentifier", httpContext.TraceIdentifier);
            };
        });
    }

    public static void UseConfiguredSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((_, configuration) => configuration.Configure());
    }
}