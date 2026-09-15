namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

public sealed record GradeLocked(int GradeId, int StudentId) : IDomainEvent;