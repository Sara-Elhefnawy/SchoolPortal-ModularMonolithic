using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using SchoolPortal.SharedKernel.Domain;
using SchoolPortal.SharedKernel.Identity.Users;

namespace SchoolPortal.SharedKernel.Interceptors;

public sealed class AuditInterceptor(
    ICurrentUser currentUser,
    ILogger<AuditInterceptor>? logger = null) 
    : SaveChangesInterceptor
{
    public void ApplyAudit(DbContext context)
    {
        var now = DateTime.UtcNow;
        var userId = currentUser.UserId;
        var entries = context.ChangeTracker.Entries<BaseEntity>().ToList();

        // Only log in development (Debug level)
        logger?.LogDebug("Applying audit to {Count} entities", entries.Count);

        // Use the generic constraint to pull only BaseEntity entries
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedByUserId = userId;
                    
                    logger?.LogDebug("Created {Type} by user {UserId} at {Now}", entry.Entity.GetType().Name, userId,
                        now);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedByUserId = userId;
                    
                    logger?.LogDebug("Updated {Type} by user {UserId} at {Now}", entry.Entity.GetType().Name, userId,
                        now);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.MarkAsDeleted(userId);

                    logger?.LogDebug("Soft-deleted {Type} by user {UserId} at {Now}", entry.Entity.GetType().Name,
                        userId, now);
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