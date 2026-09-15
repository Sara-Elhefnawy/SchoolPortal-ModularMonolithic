namespace SchoolPortal.SharedKernel.Domain;

// The DbContext override (below) needs a non-generic way to ask "did you
// raise anything?" across entities from different modules/aggregates,
// without depending on each module's concrete entity types.
public interface IHasDomainEvents
{
    bool HasDomainEvents { get; }
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}