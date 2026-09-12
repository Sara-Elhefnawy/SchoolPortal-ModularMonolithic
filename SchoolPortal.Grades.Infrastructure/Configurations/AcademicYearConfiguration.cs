using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.AcademicYears;

namespace SchoolPortal.Grades.Infrastructure.Configurations;

public sealed class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}