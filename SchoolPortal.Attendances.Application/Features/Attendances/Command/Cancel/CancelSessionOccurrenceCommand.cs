using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Cancel;

public sealed record CancelSessionOccurrenceCommand(int Id) : IRequest<Result>;