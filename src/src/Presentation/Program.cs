using Application;
using dotenv.net;
using Infrastructure.Config;
using Presentation;

// fix postgres timestamp issue
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var solutionDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent;
DotEnv.Fluent()
    .WithTrimValues()
    .WithEnvFiles($"{solutionDir}/.env")
    .WithOverwriteExistingVars()
    .Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseConfiguredSerilog();
builder.WebHost.UseStaticWebAssets();

// service registration from configurations.
ConfigurationBase.ConfigureServicesFromAssemblies(builder.Services, [
    nameof(Domain), nameof(Application), nameof(Infrastructure),
    nameof(Persistence), nameof(WebAPI), nameof(Presentation),
]);

var app = builder.Build();

app.UseConfiguredSerilogRequestLogging();
app.MigrateDatabase();

app.UseRateLimiter();
app.UseCustomHeaderMiddleware();
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
    app.UseDevelopmentMiddleware();

if (app.Environment.IsProduction())
    app.UseProductionMiddleware();

app.UseGeneralMiddleware();
app.MapEndpoints();

app.Run();