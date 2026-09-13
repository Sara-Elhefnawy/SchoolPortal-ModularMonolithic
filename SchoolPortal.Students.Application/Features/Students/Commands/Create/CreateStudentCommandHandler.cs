using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Domain.Addresses;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Create;

public sealed class CreateStudentCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<CreateStudentCommand,Result<int>>
{
    public async Task<Result<int>> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Students.AnyAsync(s => s.Email == normalizedEmail, ct);
        if (emailExists)
            return Result<int>.Failure(StudentErrors.EmailAlreadyExists);

        var addressResult = Address.Create(request.City, request.State, request.ZipCode, request.State,
            request.BuildingNumber);
        if (addressResult.IsFailure)
            return Result<int>.Failure(addressResult.Error);
        
        var studentResult  = Student.Create(request.Name, request.Email, request.PhoneNumber, addressResult.Value, request.DateOfBirth, request.Gender, request.AdmissionDate);
        if (studentResult .IsFailure)
            return Result<int>.Failure(studentResult.Error);
        
        dbContext.Students.Add(studentResult.Value);
        await dbContext.SaveChangesAsync(ct);

        return Result<int>.Success(studentResult.Value.Id);
    }
}