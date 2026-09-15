using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using SchoolPortal.SharedKernel.Domain;

namespace SchoolPortal.SharedKernel.Interceptors;

public sealed class AuditInterceptor(ILogger<AuditInterceptor>? logger = null) : SaveChangesInterceptor
{
    public void ApplyAudit(DbContext context)
    {
        var now = DateTime.UtcNow;
        var entries = context.ChangeTracker.Entries<BaseEntity>().ToList();

        // Only log in development (Debug level)
        logger?.LogDebug("Applying audit to {Count} entities", entries.Count);

        // Use the generic constraint to pull only BaseEntity entries
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = now;
                    logger?.LogDebug("Set CreatedAt for {Type} to {Now}",
                        entry.Entity.GetType().Name, now);
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
                    logger?.LogDebug("Set UpdatedAt for {Type} to {Now}",
                        entry.Entity.GetType().Name, now);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.MarkAsDeleted();
                    logger?.LogDebug("Soft-deleted {Type} at {Now}",
                        entry.Entity.GetType().Name, now);
                    break;
            }
        }
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        if (eventData.Context is not null)
            ApplyAudit(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, ct);
    }
}