using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Infrastructure.Persistence.Configurations;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne<Student>().WithMany().HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Restrict);

        // CourseSectionId is cross-module (Grades) — no FK, checked via
        // whichever directory interface Students ends up calling into Grades with.
        
        // One enrollment per student per course section.
        builder.HasIndex(e => new { e.StudentId, e.CourseSectionId }).IsUnique();

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}