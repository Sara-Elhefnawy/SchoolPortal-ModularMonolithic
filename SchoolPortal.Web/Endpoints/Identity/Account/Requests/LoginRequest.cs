namespace SchoolPortal.Web.Endpoints.Identity.Account.Requests;

public sealed record LoginRequest(
    string Email,
    string Password);