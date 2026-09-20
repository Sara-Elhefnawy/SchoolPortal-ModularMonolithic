namespace SchoolPortal.SharedKernel.Domain.Events.Grades.Terms;

public sealed record FirstTermEnded(int TermId, DateOnly EndDate) : IDomainEvent;