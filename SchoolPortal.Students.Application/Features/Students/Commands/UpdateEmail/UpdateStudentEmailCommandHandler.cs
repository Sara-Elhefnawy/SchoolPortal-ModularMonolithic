using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Dtos;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateEmail;

public class UpdateStudentEmailCommandHandler(
    IStudentDirectory directory,
    IStudentsUnitOfWork unitOfWork) 
    : IRequestHandler<UpdateStudentEmailCommand, Result<StudentSummaryDto>>
{
    public async Task<Result<StudentSummaryDto>> Handle(UpdateStudentEmailCommand request, CancellationToken ct)
    {
        var student = await directory.GetByIdAsync(request.StudentId, ct);
        if (student is null)
            return Result<StudentSummaryDto>.Failure(StudentErrors.NotFound);

        var emailExists = await directory.EmailExistsAsync(request.Email, ct);
        if (emailExists)
            return Result<StudentSummaryDto>.Failure(StudentErrors.EmailAlreadyExists);

        var updateResult = student.UpdateEmail(request.Email);
        if (updateResult.IsFailure)
            return Result<StudentSummaryDto>.Failure(updateResult.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result<StudentSummaryDto>.Success(new StudentSummaryDto(request.StudentId, student.Name, student.Email));
    }
}