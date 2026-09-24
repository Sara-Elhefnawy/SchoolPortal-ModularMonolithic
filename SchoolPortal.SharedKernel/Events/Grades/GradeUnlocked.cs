namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

public sealed record GradeUnlocked(int GradeId, int StudentId) : IDomainEvent;