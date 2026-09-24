using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Query.GetAll;

public sealed record GetAllAcademicYearsQuery : IRequest<Result<IReadOnlyList<GetAllAcademicYearsQueryDto>>>;