using System.ComponentModel;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class DomainEntityConfiguration : IEntityTypeConfiguration<IAggregateRoot>
{
    public void Configure(EntityTypeBuilder<IAggregateRoot> builder)
    {
        builder.Ignore(e => e.DomainEvents);
    }
}