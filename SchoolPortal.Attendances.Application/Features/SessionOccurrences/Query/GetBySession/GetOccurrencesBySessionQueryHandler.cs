using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Query.GetBySession;

public class GetOccurrencesBySessionQueryHandler(ISessionOccurrenceDirectory sessions)
    : IRequestHandler<GetOccurrencesBySessionQuery, Result<IReadOnlyList<GetOccurrencesBySessionQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetOccurrencesBySessionQueryDto>>> Handle(
        GetOccurrencesBySessionQuery request,
        CancellationToken ct = default)
    {
        var list = await sessions.GetOccurrencesAsync(request.SessionId, ct);

        return Result<IReadOnlyList<GetOccurrencesBySessionQueryDto>>.Success(list);
    }
}