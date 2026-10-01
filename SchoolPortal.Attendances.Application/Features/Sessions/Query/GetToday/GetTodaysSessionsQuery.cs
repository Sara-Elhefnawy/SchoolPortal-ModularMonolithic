using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Query.GetToday;

public sealed record GetTodaysSessionsQuery
    : IRequest<Result<IReadOnlyList<GetTodaysSessionsQueryDto>>>;