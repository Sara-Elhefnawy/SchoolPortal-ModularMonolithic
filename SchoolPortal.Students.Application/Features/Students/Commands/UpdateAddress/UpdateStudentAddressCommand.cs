using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Dtos;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateAddress;

public record UpdateStudentAddressCommand(int StudentId, string City, string Country, string State, string ZipCode, int BuildingNumber) :  IRequest<Result<StudentSummaryDto>>;