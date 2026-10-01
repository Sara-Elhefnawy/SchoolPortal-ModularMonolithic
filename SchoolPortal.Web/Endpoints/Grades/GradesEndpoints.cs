using SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;
using SchoolPortal.Grades.Application.Features.AcademicYears.Command.MarkAsCurrent;
using SchoolPortal.Grades.Application.Features.AcademicYears.Query.GetAll;
using SchoolPortal.Grades.Application.Features.Assessments.Command.Create;
using SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;
using SchoolPortal.Grades.Application.Features.Courses.Command.Create;
using SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;
using SchoolPortal.Grades.Application.Features.CourseSections.Command.Create;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;
using SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;
using SchoolPortal.Grades.Application.Features.Grades.Command.Create;
using SchoolPortal.Grades.Application.Features.Grades.Command.Draft;
using SchoolPortal.Grades.Application.Features.Grades.Command.Lock;
using SchoolPortal.Grades.Application.Features.Grades.Command.Publish;
using SchoolPortal.Grades.Application.Features.Grades.Command.ReviseScore;
using SchoolPortal.Grades.Application.Features.Grades.Command.Submit;
using SchoolPortal.Grades.Application.Features.Grades.Command.Unlock;
using SchoolPortal.Grades.Application.Features.Grades.Command.Unpublish;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetById;
using SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;
using SchoolPortal.Grades.Application.Features.Terms.Command.Create;
using SchoolPortal.Grades.Application.Features.Terms.Command.End;
using SchoolPortal.Grades.Application.Features.Terms.Query.GetByAcademicYear;
using SchoolPortal.SharedKernel.Identity;
using SchoolPortal.SharedKernel.Identity.Contracts;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Students.Application.Features.Students.Query.GetNamesByIds;
using SchoolPortal.Web.Endpoints.Grades.Requests;

namespace SchoolPortal.Web.Endpoints.Grades;

public static class GradesEndpoints
{
    public static void MapGradesEndpoints(this IEndpointRouteBuilder app)
    {
        MapAcademicYearEndpoints(app);
        MapTermEndpoints(app);
        MapCourseEndpoints(app);
        MapCourseSectionEndpoints(app);
        MapAssessmentEndpoints(app);
        MapGradeEndpoints(app);
        MapRankingEndpoints(app);
    }

    private static void MapAcademicYearEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/academic-years").WithTags("Academic Years");
        group.MapGet("/", GetAcademicYears).RequireAuthorization(Permissions.Grades.View);
        group.MapPost("/", CreateAcademicYear).RequireAuthorization(Permissions.Grades.Create);

        group.MapPost("/{id:int}/mark-current", MarkAcademicYearAsCurrent)
            .RequireAuthorization(Permissions.Grades.Update);
    }

    private static void MapTermEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/terms").WithTags("Terms");
        group.MapGet("/", GetTerms).RequireAuthorization(Permissions.Grades.View);
        group.MapPost("/", CreateTerm).RequireAuthorization(Permissions.Grades.Create);

        group.MapPost("/{id:int}/end", EndTerm).RequireAuthorization(Permissions.Grades.Update);
    }

    private static void MapCourseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses").WithTags("Courses");
        group.MapGet("/", GetCourses).RequireAuthorization(Permissions.Grades.View);
        group.MapPost("/", CreateCourse).RequireAuthorization(Permissions.Grades.Create);
    }

    private static void MapCourseSectionEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/course-sections").WithTags("Course Sections");
        group.MapGet("/", GetCourseSections).RequireAuthorization(Permissions.Grades.View);
        group.MapPost("/", CreateCourseSection).RequireAuthorization(Permissions.Grades.Create);
    }

    private static void MapAssessmentEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assessments").WithTags("Assessments");

        group.MapGet("/", GetAssessments).RequireAuthorization(Permissions.Grades.View);
        group.MapPost("/", CreateAssessment).RequireAuthorization(Permissions.Grades.Create);
    }

    private static void MapGradeEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/grades").WithTags("Grades");

        // Reads: any authenticated user. Scoping is enforced in the handlers below.
        group.MapGet("/", GetGrades).RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.Teacher));
        group.MapGet("/{id:int}", GetGradeById).RequireAuthorization();

        // Student self-service: only their own rows. Available to anyone authenticated,
        // the handler forces the student scope.
        group.MapGet("/me", GetMyGrades).RequireAuthorization();

        group.MapPost("/", CreateGrade).RequireAuthorization(Permissions.Grades.Create);

        group.MapPost("/{id:int}/submit", SubmitGrade).RequireAuthorization(Permissions.Grades.Update);

        group.MapPut("/{id:int}/score", ReviseGradeScore).RequireAuthorization(Permissions.Grades.Update);

        group.MapPost("/{id:int}/publish", PublishGrade)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPost("/{id:int}/unpublish", UnpublishGrade)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPost("/{id:int}/lock", LockGrade).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPost("/{id:int}/unlock", UnlockGrade).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPost("/{id:int}/draft", DraftGrade).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));
    }

    private static void MapRankingEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/course-sections").WithTags("Rankings");

        group.MapGet("/{courseSectionId:int}/rankings", GetClassRankings).RequireAuthorization(Permissions.Grades.View);
    }

    private static async Task<IResult> GetAcademicYears(IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetAllAcademicYearsQuery(), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetTerms(int academicYearId, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetTermsByAcademicYearQuery(academicYearId), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetCourses(IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetAllCoursesQuery(), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetCourseSections(int termId, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetCourseSectionsByTermQuery(termId), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetAssessments(int courseSectionId, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetAssessmentsBySectionQuery(courseSectionId), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetGrades(int assessmentId, ICurrentUser currentUser, IMediator m,
        CancellationToken ct)
    {
        var r = await m.Send(new GetGradesByAssessmentQuery(assessmentId), ct);
        if (r.IsFailure) return Results.BadRequest(r.Error);

        var names = await m.Send(new GetStudentNamesByIdsQuery(r.Value.Select(g => g.StudentId).Distinct().ToArray()),
            ct);
        var lookup = names.IsSuccess ? names.Value : new Dictionary<int, string>();

        return Results.Ok(r.Value.Select(g =>
            g with { StudentName = lookup.GetValueOrDefault(g.StudentId, "Unknown student") }));
    }

    private static async Task<IResult> GetGradeById(int id, ICurrentUser currentUser, HttpContext http,
        IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetGradeByIdQuery(id), ct);
        if (result.IsFailure) return Results.NotFound(result.Error);

        var grade = result.Value;

        var isStudent = http.User.IsInRole(Roles.Admin) || http.User.IsInRole(Roles.Teacher);
        if (!isStudent && grade.StudentId != currentUser.StudentId)
            return Results.NotFound();

        var names = await mediator.Send(new GetStudentNamesByIdsQuery([grade.StudentId]), ct);
        var studentName = names.IsSuccess
            ? names.Value.GetValueOrDefault(grade.StudentId, "Unknown student")
            : "Unknown student";

        return Results.Ok(grade with { StudentName = studentName });
    }

    private static async Task<IResult> GetMyGrades(ICurrentUser currentUser, IMediator m, CancellationToken ct, HttpContext httpContext)
    {
        Console.WriteLine($"Claims = {currentUser.StudentId}, All Claims: {string.Join(", ", httpContext.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
        
        if (currentUser.StudentId is not int studentId)
            return Results.Forbid();

        var r = await m.Send(new GetGradesForStudentQuery(studentId), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> GetClassRankings(int courseSectionId, IMediator m, CancellationToken ct)
    {
        var r = await m.Send(new GetCourseSectionRankingsQuery(courseSectionId), ct);

        return r.IsSuccess ? Results.Ok(r.Value) : Results.BadRequest(r.Error);
    }

    private static async Task<IResult> CreateAcademicYear(CreateAcademicYearCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/academic-years/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> MarkAcademicYearAsCurrent(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new MarkAcademicYearAsCurrentCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateTerm(CreateTermCommand command, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/terms/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> EndTerm(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new EndTermCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateCourse(CreateCourseCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/courses/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateCourseSection(CreateCourseSectionCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/course-sections/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateAssessment(CreateAssessmentCommand command, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/assessments/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateGrade(CreateGradeCommand command, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/grades/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> SubmitGrade(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new SubmitGradeCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> PublishGrade(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new PublishGradeCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> LockGrade(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new LockGradeCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UnlockGrade(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new UnlockGradeCommand(id), ct);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DraftGrade(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new DraftGradeCommand(id), ct);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ReviseGradeScore(int id, ReviseGradeScoreRequest body, IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ReviseGradeScoreCommand(id, body.NewScore), ct);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UnpublishGrade(int id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new UnpublishGradeCommand(id), ct);

        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
    }
}