using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;

public sealed record CreateSessionCommand(int CourseSectionId, DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime) : IRequest<Result<int>>;