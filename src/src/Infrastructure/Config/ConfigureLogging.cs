using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Config;

[EditorBrowsable(EditorBrowsableState.Never)]
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
    private const string OutputFormat =
        "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}";

    public static LoggerConfiguration Configure(this LoggerConfiguration config)
    {
        var logPath = Environment.GetEnvironmentVariable("LOG__PATH")
            ?? throw new Exception("LOG__PATH is not set");

        var seqApiKey = Environment.GetEnvironmentVariable("SEQ__API_KEY")
            ?? throw new Exception("SEQ__API_KEY is not set");

        return config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithAssemblyName()
            .WriteTo.Debug()
            .WriteTo.Console(outputTemplate: OutputFormat)
            .WriteTo.Seq("http://seq:5341", apiKey: seqApiKey)
            .WriteTo.File(logPath,
                outputTemplate: OutputFormat,
                flushToDiskInterval: TimeSpan.FromSeconds(10),
                fileSizeLimitBytes: 100_000_000,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day);
    }

    public static void UseConfiguredSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.IncludeQueryInRequestPath = true;
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms\n" +
                                      "UserId: {UserId}\n" +
                                      "Host: {RequestHost}\n" +
                                      "UserAgent: {RequestUserAgent}";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("UserId", "NaN");

                if (httpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid) is { Value: var id })
                    diagnosticContext.Set("UserId", id);

                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestUserAgent", (string?)httpContext.Request.Headers.UserAgent);
            };
        });
    }

    public static void UseConfiguredSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((_, configuration) => configuration.Configure());
    }
}