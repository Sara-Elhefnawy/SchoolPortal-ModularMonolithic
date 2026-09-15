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
using SchoolPortal.Web.Endpoints.Grades.Requests;

namespace SchoolPortal.Web.Endpoints.Grades;

public static class GradesEndpoints
{
    public static void MapGradesEndpoints(this IEndpointRouteBuilder app)
    {
        var years = app.MapGroup("/api/academic-years").WithTags("AcademicYears");
        years.MapPost("/", async (CreateAcademicYearCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(cmd, ct);
            return r.IsSuccess ? Results.Created($"/api/academic-years/{r.Value}", r.Value) : Results.BadRequest(r.Error);
        });
        years.MapPost("/{id:int}/mark-current", async (int id, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new MarkAcademicYearAsCurrentCommand(id), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });

        var terms = app.MapGroup("/api/terms").WithTags("Terms");
        terms.MapPost("/", async (CreateTermCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(cmd, ct);
            return r.IsSuccess ? Results.Created($"/api/terms/{r.Value}", r.Value) : Results.BadRequest(r.Error);
        });
        
        terms.MapPost("/{id:int}/end", async (int id, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new EndTermCommand(id), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });

        var courses = app.MapGroup("/api/courses").WithTags("Courses");
        courses.MapPost("/", async (CreateCourseCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(cmd, ct);
            return r.IsSuccess ? Results.Created($"/api/courses/{r.Value}", r.Value) : Results.BadRequest(r.Error);
        });

        var sections = app.MapGroup("/api/course-sections").WithTags("CourseSections");
        sections.MapPost("/", async (CreateCourseSectionCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(cmd, ct);
            return r.IsSuccess ? Results.Created($"/api/course-sections/{r.Value}", r.Value) : Results.BadRequest(r.Error);
        });

        var assessments = app.MapGroup("/api/assessments").WithTags("Assessments");
        assessments.MapPost("/", async (CreateAssessmentCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(cmd, ct);
            return r.IsSuccess ? Results.Created($"/api/assessments/{r.Value}", r.Value) : Results.BadRequest(r.Error);
        });

        var grades = app.MapGroup("/api/grades").WithTags("Grades");
        grades.MapPost("/", async (CreateGradeCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(cmd, ct);
            return r.IsSuccess ? Results.Created($"/api/grades/{r.Value}", r.Value) : Results.BadRequest(r.Error);
        });
        grades.MapPost("/{id:int}/submit", async (int id, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new SubmitGradeCommand(id), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });
        grades.MapPost("/{id:int}/publish", async (int id, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new PublishGradeCommand(id), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });
        grades.MapPost("/{id:int}/lock", async (int id, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new LockGradeCommand(id), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });
        
        grades.MapPut("/{id:int}/score", async (
            int id, ReviseGradeScoreRequest body, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new ReviseGradeScoreCommand(id, body.NewScore), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });

        grades.MapPost("/{id:int}/unpublish", async (int id, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new UnpublishGradeCommand(id), ct);
            return r.IsSuccess ? Results.NoContent() : Results.BadRequest(r.Error);
        });

        var rankings = app.MapGroup("/api/course-sections").WithTags("Rankings");
        rankings.MapGet("/{courseSectionId:int}/rankings", async (
            int courseSectionId, IGradeReportingQueries reporting, CancellationToken ct) =>
        {
            var results = await reporting.GetClassRankingsAsync(courseSectionId, ct);
            return Results.Ok(results);
        });
    }
}