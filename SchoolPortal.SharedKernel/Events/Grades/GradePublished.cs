namespace SchoolPortal.SharedKernel.Domain.Events.Grades;

// Deliberately carries only IDs, not the full Grade or Student entity.
// Notifications shouldn't receive a live, trackable Grades-module entity
//      that would leak the Grades module's persistence model across the
//      boundary the whole event-driven approach exists to protect.
public sealed record GradePublished(int GradeId, int StudentId, int AssessmentId) : IDomainEvent;