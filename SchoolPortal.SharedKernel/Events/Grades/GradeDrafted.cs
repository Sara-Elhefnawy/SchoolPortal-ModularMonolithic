namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

public sealed record GradeDrafted(int GradeId, int StudentId) : IDomainEvent;