namespace SchoolPortal.SharedKernel.Domain.Events.Students;

public sealed record StudentGraduated(int StudentId, DateOnly GraduationDate) : IDomainEvent;