using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Application.Dtos;
using SchoolPortal.Students.Domain.Addresses;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateAddress;

public sealed class UpdateStudentAddressCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<UpdateStudentAddressCommand, Result<StudentSummaryDto>>
{
    public async Task<Result<StudentSummaryDto>> Handle(UpdateStudentAddressCommand request, CancellationToken ct)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken: ct);
        if (student is null)
            return Result<StudentSummaryDto>.Failure(StudentErrors.NotFound);

        var addressResult = Address.Create(request.City, request.State, request.ZipCode, request.Country,
            request.BuildingNumber);
        if  (addressResult.IsFailure)
            return Result<StudentSummaryDto>.Failure(addressResult.Error);

        var updateResult = student.UpdateAddress(addressResult.Value);
        if (updateResult.IsFailure)
            return Result<StudentSummaryDto>.Failure(updateResult.Error);

        await dbContext.SaveChangesAsync(ct);

        return Result<StudentSummaryDto>.Success(new StudentSummaryDto(request.StudentId, student.Name, student.Email));
    }
}