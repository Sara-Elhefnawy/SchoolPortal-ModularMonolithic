using MediatR;
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

        // 2. Capture the events BEFORE SaveChangesAsync.
        //    The event factories are evaluated here, after all domain changes
        //    have happened but before the event queue is cleared.
        var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        // 3. Persist the actual database changes first.
        var result = await base.SaveChangesAsync(ct);

        // 4. The database write succeeded, so the events can now be published.
        foreach (var entity in entitiesWithEvents)
            entity.ClearDomainEvents();

        // 5. Notify all MediatR handlers.
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        return result;
    }
}