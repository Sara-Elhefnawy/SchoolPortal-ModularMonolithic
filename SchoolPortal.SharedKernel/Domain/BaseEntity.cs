namespace SchoolPortal.SharedKernel.Domain;

public abstract class BaseEntity : IHasDomainEvents
{
    // Store *how to build* each event, not the built event.
    // A Func<IDomainEvent> is a small piece of code we can run later, and
    //      because it's a lambda closing over `this` (the entity), calling it
    //      later re-reads whatever Id currently is at THAT moment
    //          not whatever it was when RaiseEvent was called.
    private readonly List<Func<IDomainEvent>> _domainEventFactories = [];
    
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // int — matches ApplicationUser : IdentityUser<int>.Id
    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }
    public int? DeletedByUserId { get; set; }
    
    // Needed so ModuleDbContext can check "does this entity have pending events?"
    // BEFORE save, without actually invoking the factories yet
    // (invoking them early would just reproduce the zero-Id bug one line later, at a different call site).
    public bool HasDomainEvents => _domainEventFactories.Count > 0;

    // This is where the factories actually run. Called by ModuleDbContext strictly AFTER
    // base.SaveChangesAsync() succeeds, by which point EF Core has written the real,
    // DB-generated Id back onto this same tracked entity instance
    //      so Id inside each lambda is correct now.
    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEventFactories.Select(factory => factory()).ToList().AsReadOnly();
    
    public void MarkAsDeleted(int? userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    protected void RaiseEvent(Func<IDomainEvent> eventFactory) => _domainEventFactories.Add(eventFactory);

    // Called by the DbContext after a successful save + publish, not by application code.
    // If we cleared eagerly (e.g. right after RaiseEvent), a failed SaveChangesAsync
    //      would silently lose the event with no way to retry
    //          the entity would look like it never raised anything.
    public void ClearDomainEvents() => _domainEventFactories.Clear();
}