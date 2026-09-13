using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.Courses;

namespace SchoolPortal.Grades.Infrastructure.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
        
        // 4,1 covers "3.0 credit hours" style values without over-provisioning.
        builder.Property(c => c.CreditHours).HasPrecision(4, 1).IsRequired();

        builder.HasIndex(c => c.Code).IsUnique();
        
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}