using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Domain.Enrollments;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Complete;

public sealed class CompleteEnrollmentCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<CompleteEnrollmentCommand, Result>
{
    public async Task<Result> Handle(CompleteEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await dbContext.Enrollments.FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, ct);
        if (enrollment is null)
            return Result.Failure(EnrollmentErrors.NotFound);

        var result = enrollment.Complete();
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}