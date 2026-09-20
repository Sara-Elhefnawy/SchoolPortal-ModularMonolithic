namespace SchoolPortal.Web.Endpoints.Identity.Account.Requests;

public sealed record SignupRequest(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword);