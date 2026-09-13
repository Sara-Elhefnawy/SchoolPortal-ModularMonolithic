using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;

public sealed class WithdrawEnrollmentCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<WithdrawEnrollmentCommand, Result>
{
    public async Task<Result> Handle(WithdrawEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, ct);
        if (enrollment is null)
            return Result.Failure(EnrollmentErrors.NotFound);

        var result = enrollment.Withdraw();
        if (result.IsFailure)
            return Result.Failure(result.Error);
        
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}