using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetById;

public sealed record GetGradeByIdQuery(int GradeId) : IRequest<Result<GetGradeByIdQueryDto>>;