namespace SchoolPortal.SharedKernel.Domain.Events.Students;

// Lives in SharedKernel, not Students.Domain, specifically so other modules
// (Notifications) can react to it without referencing Students at all.
public sealed record StudentPhoneNumberUpdated(int StudentId, string NewPhone) : IDomainEvent;