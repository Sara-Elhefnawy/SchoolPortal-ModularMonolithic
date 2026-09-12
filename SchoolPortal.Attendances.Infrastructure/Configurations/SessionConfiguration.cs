using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Attendances.Domain.Session;

namespace SchoolPortal.Attendances.Infrastructure.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(s => s.DayOfWeek).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(s => s.StartTime).HasColumnType("time");
        builder.Property(s => s.EndTime).HasColumnType("time");

        // One session per course section per day+start time — stops a
        // teacher accidentally scheduling the same slot twice.
        builder.HasIndex(s => new { s.CourseSectionId, s.DayOfWeek, s.StartTime }).IsUnique();

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}