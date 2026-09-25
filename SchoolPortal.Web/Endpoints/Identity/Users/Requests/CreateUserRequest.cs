namespace SchoolPortal.Web.Endpoints.Identity.Users.Requests;

public record CreateUserRequest(string Email, string Password, string Name, string Role);