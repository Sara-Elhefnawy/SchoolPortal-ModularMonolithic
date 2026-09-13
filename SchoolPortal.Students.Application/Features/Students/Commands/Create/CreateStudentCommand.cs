using MediatR;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Domain.enums;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Create;

public record CreateStudentCommand(
    string Name,  
    string Email, 
    string PhoneNumber, 
    DateOnly DateOfBirth, 
    Gender Gender,
    DateOnly AdmissionDate,
    string City,
    string State,
    string ZipCode,
    string Country,
    int BuildingNumber) : IRequest<Result<int>>;