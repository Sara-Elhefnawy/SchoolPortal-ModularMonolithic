namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

public sealed record GradeCreated(int StudentId, int AssessmentId, decimal Score) : IDomainEvent;