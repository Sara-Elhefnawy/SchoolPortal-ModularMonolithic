namespace SchoolPortal.Web.Endpoints.Admins.Requests;

public sealed record CreateUserRequest(string Name, string Email, string Password, string Role);