namespace SchoolPortal.SharedKernel.Domain;

// Base class for any entity that wants to raise domain events (Student, Attendance, etc.
// will inherit this). It does NOT publish events itself — it only holds them until
// something in Infrastructure comes and collects them after a successful save.
// This keeps Domain layer free of any dependency on MediatR's publisher, which Domain
// is not allowed to depend on.
public abstract class Entity
{
// Private + List (not IReadOnlyCollection) so ONLY this class can add to it.
    // Outside code can only read it via the DomainEvents property below, never push
    // into it directly — that would let anyone forge events, which we don't want.
    private readonly List<IDomainEvent> _domainEvents = [];

    // Public read-only view for Infrastructure to inspect after SaveChanges.
    // .AsReadOnly() wraps the list so callers can't cast back and mutate it.
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // "protected" = only this class and classes that inherit it (Student, Attendance...)
    // can call this. External code (handlers, controllers) cannot call RaiseDomainEvent
    // directly — only the entity itself decides when something domain-significant happened.
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // Called by Infrastructure right after events are read and published, so the same
    // event doesn't get published twice if SaveChanges is somehow called again on the
    // same tracked entity instance later in the same request.
    public void ClearDomainEvents() => _domainEvents.Clear();}