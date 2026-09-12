using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.CourseSections;

namespace SchoolPortal.Grades.Infrastructure.Configurations;

public sealed class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.MaximumScore).HasColumnType("decimal(6,2)");
        builder.Property(a => a.Weight).HasColumnType("decimal(4,3)");
        
        builder.HasOne<CourseSection>().WithMany().HasForeignKey(a => a.CourseSectionId).OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}