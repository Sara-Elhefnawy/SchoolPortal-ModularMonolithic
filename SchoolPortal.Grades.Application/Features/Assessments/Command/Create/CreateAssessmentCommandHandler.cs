using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Assessments.Command.Create;

public sealed class CreateAssessmentCommandHandler(
    IAssessmentDirectory assessmentDirectory, 
    ICourseSectionDirectory sectionDirectory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<CreateAssessmentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAssessmentCommand request, CancellationToken ct)
    {
        var sectionExists = await sectionDirectory.GetByIdAsync(request.CourseSectionId, ct) is not null;
        if (!sectionExists)
            return Result<int>.Failure(CourseSectionErrors.NotFound);

        // decimal arithmetic is exact here (no float rounding surprises), so a
        // direct > 1m comparison is safe — no epsilon needed the way you'd want
        // one with double/float.
        var existingAssessments = await assessmentDirectory.GetBySectionAsync(request.CourseSectionId, ct);
        var totalWeight = existingAssessments.Sum(a => a.Weight) + request.Weight;

        if (totalWeight > 1m)
            return Result<int>.Failure(AssessmentErrors.WeightExceedsSectionTotal);

        var assessmentResult = Assessment.Create(
            request.CourseSectionId, request.Name, request.Type, request.MaximumScore, request.Weight, request.Date);
        if (assessmentResult.IsFailure)
            return Result<int>.Failure(assessmentResult.Error);

        assessmentDirectory.Add(assessmentResult.Value);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<int>.Success(assessmentResult.Value.Id);
    }
}