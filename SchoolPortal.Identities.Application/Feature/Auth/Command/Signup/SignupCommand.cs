using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;

public sealed record SignupCommand(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result>;