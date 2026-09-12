using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Domain.Terms;

namespace SchoolPortal.Grades.Infrastructure.Configurations;

public sealed class TermConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        
        builder.HasOne<AcademicYear>().WithMany().HasForeignKey(t => t.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}