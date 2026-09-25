using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Query.GetBySession;

public sealed record GetOccurrencesBySessionQuery(int SessionId) : IRequest<Result<IReadOnlyList<GetOccurrencesBySessionQueryDto>>>;