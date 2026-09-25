using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetById;

public sealed record GetStudentByIdQuery(int Id) : IRequest<Result<Student>>;