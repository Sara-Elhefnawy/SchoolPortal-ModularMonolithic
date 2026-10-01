using SchoolPortal.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Domain;

namespace SchoolPortal.SharedKernel.Persistence;

public abstract class ModuleDbContext(DbContextOptions options, IPublisher publisher) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1. Capture entities that currently have domain events.
        var entitiesWithEvents = ChangeTracker.Entries<IHasDomainEvents>().Select(e => e.Entity)
            .Where(e => e.HasDomainEvents).ToList();
        
        // 3. Persist the actual database changes first.
        var result = await base.SaveChangesAsync(ct);

        // 2. Capture the events BEFORE SaveChangesAsync.
        var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        // 4. The database write succeeded, so the events can now be published.
        //    Cleared BEFORE publishing so a handler that saves on this same context
        //    doesn't re-publish the events we're about to dispatch.
        foreach (var entity in entitiesWithEvents)
            entity.ClearDomainEvents();

        // 5. Notify every INotificationHandler registered for each event's runtime type.
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        return result;
    }
}