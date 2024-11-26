using System.Reflection;
using Application.Common;
using Application.Services;
using Domain.Aggregates;
using Generated;
using Microsoft.EntityFrameworkCore;
using Persistence.Interceptors;
using Persistence.ValueConverters;

namespace Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ConvertDomainEventsToOutboxMessagesInterceptor convertDomainEventsToOutboxMessagesInterceptor)
    : DbContext(options), IAppDbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.Load(nameof(Persistence)));
        base.OnModelCreating(builder);
        SnakeCaseRename(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(convertDomainEventsToOutboxMessagesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.ConfigureGeneratedConverters();

        builder
            .Properties<DateTime>()
            .HaveConversion<DateTimeUtcValueConverter>();

        builder
            .Properties<Ulid>()
            .HaveConversion<UlidToStringConverter>();

        base.ConfigureConventions(builder);
    }

    private static void SnakeCaseRename(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var entityTableName = entity.GetTableName()!
                .Replace("AspNet", string.Empty)
                .TrimEnd('s')
                .ToSnakeCase();

            entity.SetTableName(entityTableName);

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()!.ToSnakeCase());

            foreach (var key in entity.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName()!.ToSnakeCase());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()!.ToSnakeCase());
        }
    }
}