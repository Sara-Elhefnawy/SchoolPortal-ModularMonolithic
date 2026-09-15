using MediatR;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Suspend;

public sealed class SuspendStudentCommandHandler(
    IStudentDirectory directory,
    IStudentsUnitOfWork unitOfWork)
    : IRequestHandler<SuspendStudentCommand, Result>
{
    public async Task<Result> Handle(SuspendStudentCommand request, CancellationToken ct)
    {
        var student = await directory.GetByIdAsync(request.StudentId, ct);
        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        var result = student.Suspend(request.Reason);
        if (result.IsFailure)
            return result;

        // No directory.Add/Update call needed — student came from
        // FindByIdAsync, so it's already tracked. Same reasoning as
        // dropping UpdateAsync from the directory a few messages back.
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}