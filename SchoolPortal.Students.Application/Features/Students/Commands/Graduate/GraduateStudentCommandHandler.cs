using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Graduate;

public sealed class GraduateStudentCommandHandler(IStudentDirectory directory, IStudentsUnitOfWork unitOfWork)
    : IRequestHandler<GraduateStudentCommand, Result>
{
    public async Task<Result> Handle(GraduateStudentCommand request, CancellationToken ct)
    {
        var student = await directory.GetByIdAsync(request.StudentId, ct);
        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        var result = student.Graduate(request.GraduationDate);
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}