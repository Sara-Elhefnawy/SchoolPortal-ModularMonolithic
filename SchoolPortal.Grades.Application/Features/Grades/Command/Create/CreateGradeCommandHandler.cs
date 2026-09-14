using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Create;

public sealed class CreateGradeCommandHandler(IGradeDirectory gradeDirectory, IAssessmentDirectory assessmentDirectory)
    : IRequestHandler<CreateGradeCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateGradeCommand request, CancellationToken ct)
    {
        var assessment = await assessmentDirectory.FindAsync(request.AssessmentId, ct);
        if (assessment is null)
            return Result<int>.Failure(AssessmentErrors.NotFound);

        var duplicate = await gradeDirectory.FindAsync(request.StudentId, request.AssessmentId, ct) is not null;
        if (duplicate)
            return Result<int>.Failure(GradeErrors.AlreadyExists);

        var gradeResult = Grade.Create(request.StudentId, request.AssessmentId, request.Score, assessment.MaximumScore);
        if (gradeResult.IsFailure)
            return Result<int>.Failure(gradeResult.Error);

        await gradeDirectory.AddAsync(gradeResult.Value, ct);
        return Result<int>.Success(gradeResult.Value.Id);
    }
}