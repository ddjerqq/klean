using System.ComponentModel;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Username)
            .IsUnique();

        // builder.ComplexProperty(x => x.ContactInfo, contactInfoBuilder =>
        // {
        //     contactInfoBuilder.Property(x => x.PersonalId).HasColumnName("contact_id");
        //     contactInfoBuilder.Property(x => x.FullName).HasColumnName("contact_name");
        //     contactInfoBuilder.Property(x => x.Phone).HasColumnName("contact_phone");
        // });

        builder.Property(x => x.Username)
            .HasMaxLength(32);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(64);
    }
}