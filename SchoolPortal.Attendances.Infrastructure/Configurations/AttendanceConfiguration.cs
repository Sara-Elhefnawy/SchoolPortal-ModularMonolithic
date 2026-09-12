using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.Attendances.Domain.SessionOccurrence;

namespace SchoolPortal.Attendances.Infrastructure.Configurations;

public sealed class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.Property(a => a.Status).HasConversion<string>();
        
        builder.Property(a => a.Note).HasMaxLength(250);
        
        builder.HasOne<SessionOccurrence>().WithMany().HasForeignKey(a => a.SessionOccurrenceId).OnDelete(DeleteBehavior.Restrict);

        // StudentId is cross-module (Students) — no FK, checked via directory.
        
        // one record per student/course/day
        builder.HasIndex(a => new { a.StudentId, a.SessionOccurrenceId }).IsUnique();
        
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}