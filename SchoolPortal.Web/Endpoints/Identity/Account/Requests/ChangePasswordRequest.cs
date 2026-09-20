namespace SchoolPortal.Web.Endpoints.Identity.Account.Requests;

public sealed record ChangePasswordRequest(
    string Email,
    string Password,
    string NewPassword);