using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolPortal.Attendances.Domain.Sessions;
using SchoolPortal.Attendances.Domain.SessionOccurrences;

namespace SchoolPortal.Attendances.Infrastructure.Persistence.Configurations;

public sealed class SessionOccurrenceConfiguration : IEntityTypeConfiguration<SessionOccurrence>
{
    public void Configure(EntityTypeBuilder<SessionOccurrence> builder)
    {
        builder.HasOne<Session>().WithMany().HasForeignKey(o => o.SessionId).OnDelete(DeleteBehavior.Restrict);

        // One occurrence per session per calendar date — the recurring
        // template only ever produces one concrete instance per day.
        builder.HasIndex(o => new { o.SessionId, o.Date }).IsUnique();

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}