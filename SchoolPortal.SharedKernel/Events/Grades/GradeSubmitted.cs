namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

public sealed record GradeSubmitted(int GradeId, int StudentId) : IDomainEvent;