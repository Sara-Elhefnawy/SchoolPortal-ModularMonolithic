namespace SchoolPortal.SharedKernel.Domain.Events.Grades.Terms;

public sealed record FinalTermEnded(int TermId, DateOnly EndDate) : IDomainEvent;