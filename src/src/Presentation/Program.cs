using Application;
using dotenv.net;
using FluentValidation;
using Infrastructure.Config;
using Presentation;
using SerilogTracing;

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
using var _ = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests(options => options.IncomingTraceParent = IncomingTraceParent.Trust)
    .Instrument.SqlClientCommands()
    .TraceToSharedLogger();

builder.WebHost.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
builder.WebHost.UseStaticWebAssets();

// service registration from configurations.
ConfigurationBase.ConfigureServicesFromAssemblies(builder.Services, [
    nameof(Domain), nameof(Application), nameof(Infrastructure),
    nameof(Persistence), nameof(Presentation),
]);

var app = builder.Build();

app.UseConfiguredSerilogRequestLogging();
await app.MigrateDatabaseAsync();
app.UseApplicationMiddleware();

app.Run();