using MediatR;
using SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;
using SchoolPortal.Grades.Application.Features.AcademicYears.Command.MarkAsCurrent;
using SchoolPortal.Grades.Application.Features.Assessments.Command.Create;
using SchoolPortal.Grades.Application.Features.Courses.Command.Create;
using SchoolPortal.Grades.Application.Features.CourseSections.Command.Create;
using SchoolPortal.Grades.Application.Features.Grades.Command.Create;
using SchoolPortal.Grades.Application.Features.Grades.Command.Lock;
using SchoolPortal.Grades.Application.Features.Grades.Command.Publish;
using SchoolPortal.Grades.Application.Features.Grades.Command.ReviseScore;
using SchoolPortal.Grades.Application.Features.Grades.Command.Submit;
using SchoolPortal.Grades.Application.Features.Grades.Command.Unpublish;
using SchoolPortal.Grades.Application.Features.Terms.Command.Create;
using SchoolPortal.Grades.Application.Features.Terms.Command.End;
using SchoolPortal.Grades.Application.Reporting.Queries;
using SchoolPortal.SharedKernel.Identity;
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
        var group = app.MapGroup("/api/academic-years").WithTags("AcademicYears");

        group.MapPost("/", CreateAcademicYear).RequireAuthorization(Permissions.Grades.Create);

        group.MapPost("/{id:int}/mark-current", MarkAcademicYearAsCurrent)
            .RequireAuthorization(Permissions.Grades.Update);
    }

    private static void MapTermEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/terms").WithTags("Terms");

        group.MapPost("/", CreateTerm).RequireAuthorization(Permissions.Grades.Create);

        group.MapPost("/{id:int}/end", EndTerm).RequireAuthorization(Permissions.Grades.Update);
    }

    private static void MapCourseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses").WithTags("Courses");

        group.MapPost("/", CreateCourse).RequireAuthorization(Permissions.Grades.Create);
    }

    private static void MapCourseSectionEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/course-sections").WithTags("CourseSections");

        group.MapPost("/", CreateCourseSection).RequireAuthorization(Permissions.Grades.Create);
    }

    private static void MapAssessmentEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assessments").WithTags("Assessments");

        group.MapPost("/", CreateAssessment).RequireAuthorization(Permissions.Grades.Create);
    }

    private static void MapGradeEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/grades").WithTags("Grades");

        group.MapPost("/", CreateGrade).RequireAuthorization(Permissions.Grades.Create);

        group.MapPost("/{id:int}/submit", SubmitGrade).RequireAuthorization(Permissions.Grades.Update);

        group.MapPost("/{id:int}/publish", PublishGrade).RequireAuthorization(Permissions.Grades.Update);

        group.MapPost("/{id:int}/lock", LockGrade).RequireAuthorization(Permissions.Grades.Update);

        group.MapPut("/{id:int}/score", ReviseGradeScore).RequireAuthorization(Permissions.Grades.Update);

        group.MapPost("/{id:int}/unpublish", UnpublishGrade).RequireAuthorization(Permissions.Grades.Update);
    }

    private static void MapRankingEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/course-sections").WithTags("Rankings");

        group.MapGet("/{courseSectionId:int}/rankings", GetClassRankings).RequireAuthorization(Permissions.Grades.View);
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

    private static async Task<IResult> GetClassRankings(int courseSectionId, IGradeReportingQueries reporting,
        CancellationToken ct)
    {
        var results = await reporting.GetClassRankingsAsync(courseSectionId, ct);

        return Results.Ok(results);
    }
}