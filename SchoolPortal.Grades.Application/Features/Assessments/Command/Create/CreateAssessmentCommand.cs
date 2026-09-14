using MediatR;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Assessments.Command.Create;

public sealed record CreateAssessmentCommand(
    int CourseSectionId, string Name, AssessmentType Type, decimal MaximumScore, decimal Weight, DateOnly Date)
    : IRequest<Result<int>>;