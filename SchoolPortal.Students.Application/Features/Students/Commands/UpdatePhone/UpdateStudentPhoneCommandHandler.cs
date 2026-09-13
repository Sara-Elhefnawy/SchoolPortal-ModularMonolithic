using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Application.Dtos;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdatePhone;

public class UpdateStudentPhoneCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<UpdateStudentPhoneCommand, Result<StudentSummaryDto>>
{
    public async Task<Result<StudentSummaryDto>> Handle(UpdateStudentPhoneCommand request, CancellationToken ct)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken: ct);
        if (student is null)
            return Result<StudentSummaryDto>.Failure(StudentErrors.NotFound);

        var updateResult = student.UpdatePhoneNumber(request.Phone);
        if (updateResult.IsFailure)
            return Result<StudentSummaryDto>.Failure(updateResult.Error);

        await dbContext.SaveChangesAsync(ct);

        return Result<StudentSummaryDto>.Success(new StudentSummaryDto(request.StudentId, student.Name, student.Email));
    }
}