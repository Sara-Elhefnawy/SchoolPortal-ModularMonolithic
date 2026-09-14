using MediatR;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;

public sealed class WithdrawEnrollmentCommandHandler(IEnrollmentDirectory directory)
    : IRequestHandler<WithdrawEnrollmentCommand, Result>
{
    public async Task<Result> Handle(WithdrawEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await directory.FindByIdAsync(request.EnrollmentId, ct);
        if (enrollment is null)
            return Result.Failure(EnrollmentErrors.NotFound);

        var result = enrollment.Withdraw();
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await directory.UpdateAsync(ct);

        return Result.Success();
    }
}