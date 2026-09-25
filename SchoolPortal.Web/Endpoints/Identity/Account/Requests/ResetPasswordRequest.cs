namespace SchoolPortal.Web.Endpoints.Identity.Account.Requests;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword, string ConfirmPassword);