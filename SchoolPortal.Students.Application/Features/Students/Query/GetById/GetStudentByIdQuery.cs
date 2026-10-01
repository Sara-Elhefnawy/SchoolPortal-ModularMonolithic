using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetById;

public sealed record GetStudentByIdQuery(int Id) : IRequest<Result<GetStudentByIdQueryDto>>;