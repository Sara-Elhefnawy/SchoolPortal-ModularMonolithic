namespace SchoolPortal.Web.Endpoints.Identity.Account.Requests;

public sealed record SignupRequest(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword);