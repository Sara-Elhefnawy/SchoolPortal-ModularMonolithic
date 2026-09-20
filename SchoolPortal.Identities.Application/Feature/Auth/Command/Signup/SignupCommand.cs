using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Signup;

public sealed record SignupCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result>;