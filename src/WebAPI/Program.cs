using Application;
using Infrastructure;
using WebAPI;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddSingleton(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddBackgroundServices()
    .AddPersistenceServices()
    .AddWebApiServices(builder.Environment)
    .AddAuthServices();

builder.Build()
    .MigrateDatabase()
    .ConfigureWebApiMiddleware()
    .Run();