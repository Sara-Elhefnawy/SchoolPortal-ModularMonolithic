using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetById;

public sealed class GetGradeByIdQueryHandler(
    IGradeDirectory directory)
    : IRequestHandler<GetGradeByIdQuery, Result<GetGradeByIdQueryDto>>
{
    public async Task<Result<GetGradeByIdQueryDto>> Handle(GetGradeByIdQuery request, CancellationToken ct)
    {
        var grade = await directory.GetByIdAsync(request.GradeId, ct);
        if (grade is null)
            return Result<GetGradeByIdQueryDto>.Failure(GradeErrors.NotFound);
 
        var assessmentName = await directory.GetAssessmentNameAsync(grade.AssessmentId, ct) ?? "";
        
        return Result<GetGradeByIdQueryDto>.Success(
            new GetGradeByIdQueryDto(grade.Id, grade.StudentId, grade.AssessmentId, grade.Score, grade.Status.ToString(), AssessmentName: assessmentName));
    }
}