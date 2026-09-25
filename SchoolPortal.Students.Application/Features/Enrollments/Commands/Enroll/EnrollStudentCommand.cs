using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Enroll;

public sealed record EnrollStudentCommand(int StudentId, int CourseSectionId, DateOnly Date) : IRequest<Result<int>>;