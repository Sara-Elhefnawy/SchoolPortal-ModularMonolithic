using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Auth.Command.Logout;

public sealed record LogoutCommand : IRequest<Result>;