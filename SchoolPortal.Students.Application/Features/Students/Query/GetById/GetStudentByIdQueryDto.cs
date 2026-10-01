namespace SchoolPortal.Students.Application.Features.Students.Query.GetById;

public sealed record GetStudentByIdQueryDto(
    int Id, string Name, string Email, string PhoneNumber,
    DateOnly DateOfBirth, string Gender, DateOnly AdmissionDate, string Status,
    StudentAddressDto Address);
    
public sealed record StudentAddressDto(string BuildingNumber, string City, string State, string ZipCode, string Country);