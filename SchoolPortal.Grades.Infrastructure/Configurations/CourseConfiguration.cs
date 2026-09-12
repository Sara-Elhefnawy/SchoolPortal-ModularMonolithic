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
        builder.Property(c => c.CreditHours).HasColumnType("decimal(5,2)");

        builder.HasIndex(c => c.Code).IsUnique();
        
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}