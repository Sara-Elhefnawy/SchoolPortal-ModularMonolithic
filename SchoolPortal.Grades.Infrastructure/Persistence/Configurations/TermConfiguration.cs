using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Domain.Terms;

namespace SchoolPortal.Grades.Infrastructure.Persistence.Configurations;

public sealed class TermConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        
        builder.HasOne<AcademicYear>().WithMany().HasForeignKey(t => t.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        
        // Stops "Fall 2026" existing twice inside the same academic year
        // doesn't stop the same name existing in different years, which is fine
        builder.HasIndex(t => new { t.AcademicYearId, t.Name }).IsUnique();

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}