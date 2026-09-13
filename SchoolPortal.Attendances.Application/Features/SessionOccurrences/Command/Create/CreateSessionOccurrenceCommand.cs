using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;

public sealed record CreateSessionOccurrenceCommand(int SessionId, DateOnly Date) : IRequest<Result<int>>;