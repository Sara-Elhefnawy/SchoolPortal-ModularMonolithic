using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Application.Persistence;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Enroll;

public sealed class EnrollStudentCommandHandler(
    IStudentDirectory studentDirectory, 
    IEnrollmentDirectory enrollmentDirectory,
    IStudentsUnitOfWork unitOfWork)
    : IRequestHandler<EnrollStudentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(EnrollStudentCommand request, CancellationToken ct)
    {
        var studentExists = await studentDirectory.ExistsAsync(request.StudentId, ct);
        if (!studentExists)
            return Result<int>.Failure(StudentErrors.NotFound);

        /*var courseSectionExists = await courseSectionDirectory.ExistsAsync(request.CourseSectionId, ct);
        if (!courseSectionExists)
            return Result<int>.Failure(CourseSectionErrors.NotFound);*/

        var alreadyEnrolled = await enrollmentDirectory.StudentAndCourseSectionExistsAsync(request.StudentId, request.CourseSectionId, ct);
        if (alreadyEnrolled)
            return Result<int>.Failure(EnrollmentErrors.AlreadyEnrolled);

        var enrollmentResult = Enrollment.Create(request.StudentId, request.CourseSectionId, request.Date);
        if (enrollmentResult.IsFailure)
            return Result<int>.Failure(enrollmentResult.Error);

        enrollmentDirectory.Add(enrollmentResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(enrollmentResult.Value.Id);
    }
}