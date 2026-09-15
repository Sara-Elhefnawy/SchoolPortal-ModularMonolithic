namespace SchoolPortal.SharedKernel.Domain.Events.Grades.Terms;

public sealed record SecondTermEnded(int TermId, DateOnly EndDate) : IDomainEvent;