using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Attendance.Domain.Attendances;

namespace SchoolPortal.Attendance.Infrastructure.Configurations;

public sealed class AttendanceConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.Property(a => a.Status).HasConversion<string>();
        
        builder.Property(a => a.Note).HasMaxLength(500);
        
        // one record per student/course/day
        builder.HasIndex(a => new { a.StudentId, a.SessionOccurrenceId }).IsUnique();
        
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}