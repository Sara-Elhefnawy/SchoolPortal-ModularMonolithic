using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Publish;

public sealed class PublishGradeCommandHandler(
    IGradeDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<PublishGradeCommand, Result>
{
    public async Task<Result> Handle(PublishGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.GetByIdAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var publishResult = grade.Publish();
        if (publishResult.IsFailure)
            return publishResult;
        
        // grade.Publish() is what queues the GradePublished domain event that it's handler calls emailSender.SendGradePublishedEmailAsync
        // SaveChangesAsync is what dispatches it, the same way every other domain event in this project already works.
        // Grades doesn't call IEmailSender, doesn't know Notifications exists, and doesn't know an email gets sent at all
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}