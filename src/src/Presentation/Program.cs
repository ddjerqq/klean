using dotenv.net;
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
builder.WebHost.ConfigureAssemblies();

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

app.UseAppMiddleware();

app.MapEndpoints();

app.Run();