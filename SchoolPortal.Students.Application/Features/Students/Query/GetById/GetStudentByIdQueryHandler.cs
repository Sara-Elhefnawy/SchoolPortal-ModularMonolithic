using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetById;

public sealed class GetStudentByIdQueryHandler(IStudentDirectory directory)
    : IRequestHandler<GetStudentByIdQuery, Result<GetStudentByIdQueryDto>>
{
    public async Task<Result<GetStudentByIdQueryDto>> Handle(GetStudentByIdQuery request,
        CancellationToken ct = default)
    {
        var student = await directory.GetByIdAsync(request.Id, ct);
        if (student is null)
            return Result<GetStudentByIdQueryDto>.Failure(StudentErrors.NotFound);

        var a = student.Address;

        return Result<GetStudentByIdQueryDto>.Success(new GetStudentByIdQueryDto(student.Id, student.Name,
            student.Email, student.PhoneNumber, student.DateOfBirth, student.Gender.ToString(), student.AdmissionDate,
            student.Status.ToString(),
            new StudentAddressDto(Convert.ToString(a.BuildingNumber) ?? "", a.City, a.State, a.ZipCode, a.Country)));
    }
}