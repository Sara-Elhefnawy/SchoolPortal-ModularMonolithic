using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.Grades;

namespace SchoolPortal.Grades.Infrastructure.Configurations;

public sealed class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.Property(g => g.Score).HasPrecision(6, 2).IsRequired();
        builder.Property(g => g.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne<Assessment>().WithMany().HasForeignKey(g => g.AssessmentId).OnDelete(DeleteBehavior.Restrict);

        // StudentId points into the Students module's schema — deliberately NOT an FK.
        // Existence is checked by the handler via IStudentDirectory before a Grade is ever created.
        
        // One grade per student per assessment
        builder.HasIndex(g => new { g.StudentId, g.AssessmentId }).IsUnique();
        
        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}