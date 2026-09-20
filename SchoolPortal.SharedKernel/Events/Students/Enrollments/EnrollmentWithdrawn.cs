namespace SchoolPortal.SharedKernel.Domain.Events.Students.Enrollments;

public sealed record EnrollmentWithdrawn(int EnrollmentId, int StudentId, int CourseSectionId, DateOnly EffectiveDate) : IDomainEvent;