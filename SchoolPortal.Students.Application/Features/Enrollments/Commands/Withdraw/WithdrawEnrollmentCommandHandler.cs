using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;

public sealed class WithdrawEnrollmentCommandHandler(
    IEnrollmentDirectory directory,
    IStudentsUnitOfWork unitOfWork)
    : IRequestHandler<WithdrawEnrollmentCommand, Result>
{
    public async Task<Result> Handle(WithdrawEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await directory.GetByIdAsync(request.EnrollmentId, ct);
        if (enrollment is null)
            return Result.Failure(EnrollmentErrors.NotFound);

        var result = enrollment.Withdraw(DateOnly.FromDateTime(DateTime.Now));
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}