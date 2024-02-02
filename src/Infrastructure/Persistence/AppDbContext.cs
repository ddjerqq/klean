using Application.Abstractions;
using Application.Common;
using Domain.Aggregates;
using Domain.Common.Extensions;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext"/>
public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    EntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
    ConvertDomainEventsToOutboxMessagesInterceptor convertDomainEventsToOutboxMessagesInterceptor)
    : DbContext(options), IAppDbContext
{
    /// <summary>
    /// Gets the user repository
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the item repository
    /// </summary>
    public DbSet<Item> Items => Set<Item>();

    /// <summary>
    /// Gets the item type repository
    /// </summary>
    public DbSet<ItemType> ItemTypes => Set<ItemType>();

    /// <summary>
    /// Gets the outbox message repository
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
        SnakeCaseRename(builder);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditableEntitySaveChangesInterceptor);
        optionsBuilder.AddInterceptors(convertDomainEventsToOutboxMessagesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder
            .Properties<DateTime>()
            .HaveConversion<DateTimeUtcValueConverter>();

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