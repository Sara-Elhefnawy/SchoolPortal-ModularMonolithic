using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Application.Dtos;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateEmail;

public class UpdateStudentEmailCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<UpdateStudentEmailCommand, Result<StudentSummaryDto>>
{
    public async Task<Result<StudentSummaryDto>> Handle(UpdateStudentEmailCommand request, CancellationToken ct)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken: ct);
        if (student is null)
            return Result<StudentSummaryDto>.Failure(StudentErrors.NotFound);
        
        var emailExists = await dbContext.Students.AnyAsync(s => s.Email == request.Email, ct);
        if (emailExists)
            return Result<StudentSummaryDto>.Failure(StudentErrors.EmailAlreadyExists);

        var updateResult = student.UpdatePhoneNumber(request.Email);
        if (updateResult.IsFailure)
            return Result<StudentSummaryDto>.Failure(updateResult.Error);

        await dbContext.SaveChangesAsync(ct);

        return Result<StudentSummaryDto>.Success(new StudentSummaryDto(request.StudentId, student.Name, student.Email));
    }
}