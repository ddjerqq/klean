using Application;
using dotenv.net;
using FluentValidation;
using Infrastructure.Config;
using Presentation.Common;

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
// for custom languages
// ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ka");

var solutionDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent;
DotEnv.Fluent()
    .WithTrimValues()
    .WithEnvFiles($"{solutionDir}/.env")
    .WithOverwriteExistingVars()
    .Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseConfiguredSerilog();

// for serilog - seq tracing
#if !DEBUG
using var _ = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests(options => options.IncomingTraceParent = IncomingTraceParent.Trust)
    .TraceToSharedLogger();
#endif

builder.WebHost.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
builder.WebHost.UseStaticWebAssets();

// service registration from configurations.
ConfigurationBase.ConfigureServicesFromAssemblies(builder.Services, [
    nameof(Domain), nameof(Application), nameof(Infrastructure),
    nameof(Persistence), nameof(Presentation),
]);

var app = builder.Build();

#if !DEBUG
app.UseConfiguredSerilogRequestLogging();
#endif

await app.MigrateDatabaseAsync();
app.UseApplicationMiddleware();

app.Run();