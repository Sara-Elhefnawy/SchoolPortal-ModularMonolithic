using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Domain.Terms;

namespace SchoolPortal.Grades.Infrastructure.Persistence.Configurations;

public sealed class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
{
    public void Configure(EntityTypeBuilder<CourseSection> builder)
    {
        builder.HasOne<Course>().WithMany().HasForeignKey(cs => cs.CourseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Term>().WithMany().HasForeignKey(cs => cs.TermId).OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(cs => new { cs.CourseId, cs.TermId }).IsUnique();
        
        builder.HasQueryFilter(cs => !cs.IsDeleted);
    }
}