namespace SchoolPortal.SharedKernel.Domain.Events.Students.Enrollments;

public sealed record EnrollmentCompleted(int EnrollmentId, int StudentId, int CourseSectionId) : IDomainEvent;