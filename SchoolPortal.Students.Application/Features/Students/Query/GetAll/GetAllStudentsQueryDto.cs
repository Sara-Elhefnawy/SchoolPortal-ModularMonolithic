using SchoolPortal.Students.Domain.enums;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetAll;

public sealed record GetAllStudentsQueryDto(
    int Id,
    string Name,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    Gender Gender,
    DateOnly AdmissionDate);