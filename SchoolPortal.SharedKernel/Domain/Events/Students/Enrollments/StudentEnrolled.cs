namespace SchoolPortal.SharedKernel.Domain.Events.Students.Enrollments;

public sealed record StudentEnrolled(int EnrollmentId, int StudentId, int CourseSectionId) : IDomainEvent;