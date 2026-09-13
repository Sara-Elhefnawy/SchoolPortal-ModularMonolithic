using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.DbContexts;
using SchoolPortal.Students.Domain.Enrollments;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Enroll;

public sealed class EnrollStudentCommandHandler(IStudentsDbContext dbContext) : IRequestHandler<EnrollStudentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(EnrollStudentCommand request, CancellationToken ct)
    {
        var studentExists = await dbContext.Students.AnyAsync(s => s.Id == request.StudentId, ct);
        if (!studentExists)
            return Result<int>.Failure(StudentErrors.NotFound);
        
        /*var courseSectionIdExists = await dbContext.CourseSections.AnyAsync(s => s.Id == request.CourseSectionId);
        if (!courseSectionIdExists)
            return Result<int>.Failure(CourseSectionErrors.NotFound);*/
        
        // unique index for StudentId and CourseSectionId
        var alreadyEnrolled = await dbContext.Enrollments.AnyAsync(
            e => e.StudentId == request.StudentId && e.CourseSectionId == request.CourseSectionId, ct);
        if (alreadyEnrolled)
            return Result<int>.Failure(EnrollmentErrors.AlreadyEnrolled);

        var enrollmentResult = Enrollment.Create(request.StudentId, request.CourseSectionId, request.Date);
        if (enrollmentResult.IsFailure)
            return Result<int>.Failure(enrollmentResult.Error);

        dbContext.Enrollments.Add(enrollmentResult.Value);
        await dbContext.SaveChangesAsync(ct);

        return Result<int>.Success(enrollmentResult.Value.Id);
    }
}