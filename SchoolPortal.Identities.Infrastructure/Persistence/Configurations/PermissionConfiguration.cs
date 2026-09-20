using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Identities.Domain.Permissions;

namespace SchoolPortal.Identities.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);

        builder.HasIndex(x => x.Name).IsUnique();
    }
}