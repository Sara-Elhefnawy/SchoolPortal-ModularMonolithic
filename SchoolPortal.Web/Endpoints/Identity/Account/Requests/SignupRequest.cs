namespace SchoolPortal.Web.Endpoints.Identity.Account.Requests;

public sealed record SignupRequest(
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
    int BuildingNumber);