using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;

public sealed record SignupCommand(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword,
    string PhoneNumber, 
    DateOnly DateOfBirth, 
    string Gender,
    DateOnly AdmissionDate,
    string City,
    string State,
    string ZipCode,
    string Country,
    int BuildingNumber) : IRequest<Result>;