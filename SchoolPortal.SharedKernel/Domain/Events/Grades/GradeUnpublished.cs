namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

public sealed record GradeUnpublished(int GradeId, int StudentId) : IDomainEvent;