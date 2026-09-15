using MediatR;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Deactivate;

public sealed class DeactivateStudentCommandHandler(
    IStudentDirectory directory,
    IStudentsUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateStudentCommand, Result>
{
    public async Task<Result> Handle(DeactivateStudentCommand request, CancellationToken ct)
    {
        var student = await directory.GetByIdAsync(request.StudentId, ct);
        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        var result = student.Deactivate();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}