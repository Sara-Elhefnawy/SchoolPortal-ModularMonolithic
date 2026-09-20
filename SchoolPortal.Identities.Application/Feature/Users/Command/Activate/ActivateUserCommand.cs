using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Identities.Application.Feature.Users.Command.Activate;

public sealed record ActivateUserCommand(int Id) : IRequest<Result>;