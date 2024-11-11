using Application;
using Application.Services;
using Domain.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Interceptors;

namespace Persistence;

public class ConfigurePersistence : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EntitySaveChangesInterceptor>();
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<AppDbContext>(builder =>
        {
            if (IsDevelopment)
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
            }

            var dbFilePath = "DB__PATH".FromEnvRequired();
            var dbDirectory = Path.GetDirectoryName(dbFilePath)!;

            var directory = new DirectoryInfo(dbDirectory);
            if (!directory.Exists)
                directory.Create();

            builder.UseSqlite($"Data Source={dbFilePath}");
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        // delegate the IDbContext to the AppDbContext;
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddDataProtection()
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256,
                }
            )
            .SetApplicationName("klean")
            .PersistKeysToFileSystem(new DirectoryInfo("ASPNETCORE_DATAPROTECTION__PATH".FromEnvRequired()));
    }
}