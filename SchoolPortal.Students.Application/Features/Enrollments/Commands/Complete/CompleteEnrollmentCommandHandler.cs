using MediatR;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Complete;

public sealed class CompleteEnrollmentCommandHandler(IEnrollmentDirectory directory)
    : IRequestHandler<CompleteEnrollmentCommand, Result>
{
    public async Task<Result> Handle(CompleteEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await directory.FindByIdAsync(request.EnrollmentId, ct);
        if (enrollment is null)
            return Result.Failure(EnrollmentErrors.NotFound);

        var result = enrollment.Complete();
        if (result.IsFailure)
            return result;

        await directory.UpdateAsync(ct);
        return Result.Success();
    }
}