namespace SchoolPortal.Web.Endpoints.Identity.Users.Requests;

public sealed record UpdateUserRequest(
    string Email,
    string? Name,
    string? PhoneNumber);