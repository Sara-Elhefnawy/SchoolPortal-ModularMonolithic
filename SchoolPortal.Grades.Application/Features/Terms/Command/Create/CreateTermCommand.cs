using MediatR;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Terms.Command.Create;

public sealed record CreateTermCommand(
    string Name, TermType Type, DateOnly StartDate, DateOnly EndDate, int AcademicYearId) : IRequest<Result<int>>;