using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.CourseSections;

namespace SchoolPortal.Grades.Infrastructure.Persistence.Configurations;

public sealed class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.MaximumScore).HasColumnType("decimal(6,2)");
        
        // it needs 3 decimal places to represent something like 0.125 exactly rather than rounding it.
        builder.Property(a => a.Weight).HasPrecision(4, 3).IsRequired();
        
        // Deliberately NOT adding a unique/check constraint that sums of Weight per CourseSection == 1
        // SQL can't express "sum of sibling rows == X" as a column-level constraint without a trigger
        // That validation belongs in the command handler that creates/edits an Assessment
        
        builder.HasOne<CourseSection>().WithMany().HasForeignKey(a => a.CourseSectionId).OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}