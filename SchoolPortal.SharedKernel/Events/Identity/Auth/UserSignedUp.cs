using SchoolPortal.SharedKernel.Domain;

namespace SchoolPortal.SharedKernel.Events.Identity.Auth;

public sealed record UserSignedUp(
    int UserId,
    string Name,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    string Gender,
    DateOnly AdmissionDate,
    string City,
    string State,
    string ZipCode,
    string Country,
    int BuildingNumber) : IDomainEvent;